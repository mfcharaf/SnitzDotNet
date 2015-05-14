/*
####################################################################################################################
##
## SnitzMembership - SnitzMembershipProvider
##   
## Author:		Huw Reddick
## Copyright:	Huw Reddick
## based on code from Snitz Forums 2000 (c) Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## Created:		29/07/2013
## 
## The use and distribution terms for this software are covered by the 
## Eclipse License 1.0 (http://opensource.org/licenses/eclipse-1.0)
## which can be found in the file Eclipse.txt at the root of this distribution.
## By using this software in any fashion, you are agreeing to be bound by 
## the terms of this license.
##
## You must not remove this notice, or any other, from this software.  
##
#################################################################################################################### 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Security;
using System.Web.Configuration;
using System.Security.Cryptography;
using System.Configuration;
using System.Configuration.Provider;
using System.Text;
using Snitz.Entities;
using Snitz.Membership.Helpers;
using Snitz.Membership.IDal;
using SnitzConfig;



namespace Snitz.Providers
{
    [DataObject(true)]
    [Serializable]
    public sealed class SnitzMembershipProvider : MembershipProvider
    {
        private MachineKeySection _machineKey;

        /*************************************************************************
         * General settings
         *************************************************************************/

        private string _applicationName;
        public override string ApplicationName
        {
            get { return _applicationName; }
            set { _applicationName = value; }
        }

        private bool _requiresUniqeEmail;
        public override bool RequiresUniqueEmail
        {
            get { return _requiresUniqeEmail; }
        }


        /*************************************************************************
         * Private settings
         *************************************************************************/

        public string ProviderName { get; private set; }

        private TimeSpan _userIsOnlineTimeWindow;
        public TimeSpan UserIsOnlineTimeWindow
        {
            get { return new TimeSpan(0,System.Web.Security.Membership.UserIsOnlineTimeWindow,0); }
        }


        /*************************************************************************
         * Password settings
         *************************************************************************/

        private int _minRequiredNonAlphanumericCharacters;
        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters; }
        }

        private int _minRequiredPasswordLength;
        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength; }
        }

        private bool _enablePasswordReset;
        public override bool EnablePasswordReset
        {
            get { return _enablePasswordReset; }
        }

        private bool _enablePasswordRetrieval;
        public override bool EnablePasswordRetrieval
        {
            get { return _enablePasswordRetrieval; }
        }

        private int _passwordAttemptWindow;
        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow; }
        }

        private string _passwordStrengthRegularExpression;
        public override string PasswordStrengthRegularExpression
        {
            get { return _passwordStrengthRegularExpression; }
        }

        private MembershipPasswordFormat _passwordFormat;
        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }//_passwordFormat; }
        }

        private int _maxInvalidPasswordAttempts;
        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts; }
        }

        private bool _requiresQuestionAndAnswer;
        public override bool RequiresQuestionAndAnswer
        {
            get { return _requiresQuestionAndAnswer; }
        }


        /*************************************************************************
         * User related methods : create, update, unlock, delete methods.
         *************************************************************************/

        /// <summary>
        /// Creates a new user with a given set of default values
        /// </summary>
        public override MembershipUser CreateUser(string username, string password, string email,
            string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey,
            out MembershipCreateStatus status)
        {
            var args = new ValidatePasswordEventArgs(username, password, true);

            OnValidatingPassword(args);

            if (args.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            if (RequiresUniqueEmail && GetUserNameByEmail(email) != "")
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            // If no user with this name already exists
            MembershipUser mu = GetUser(username, false);
            if (mu == null || mu.UserName.ToLower() == "guest")
            {
                DateTime createdDate = DateTime.UtcNow;

                //if (PasswordFormat == MembershipPasswordFormat.Hashed)
                //{
                //    string salt = GenerateSalt();
                //    password = password + salt;
                //}

                var m = new MemberInfo {Username = username, Password = EncodePassword(password), Email = email};

                // Set the password retrieval question and answer if they are required
                if (RequiresQuestionAndAnswer)
                {
                    m.PasswordChangeKey = passwordQuestion;
                    m.ValidationKey = EncodePassword(passwordAnswer);
                }

                m.IsValid = isApproved;
                m.Status = 0;
                m.ReceiveEmails =false;
                m.TimeOffset = 0;
                m.MemberSince = createdDate;
                m.LastVisitDate = createdDate;
                m.LastPostDate = null;


                try
                {
                    Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                    dal.Add(m);
                    IRoles roleDal = Factory<IRoles>.Create("Role");
                    roleDal.AddUsersToRoles(new[] {m.Username},new[] {"all"});

                    // User creation was a success
                    status = MembershipCreateStatus.Success;

                    // Return the newly craeted user
                    return GetUserFromMember(m);
                }
                catch
                {
                    // Something was wrong and the user was rejected
                    status = MembershipCreateStatus.UserRejected;
                }
            }
            else
            {
                // There is already a user with this name
                status = MembershipCreateStatus.DuplicateUserName;
            }

            // Something went wrong if we got this far without some sort of status or retun
            if (status != MembershipCreateStatus.UserRejected && status != MembershipCreateStatus.DuplicateUserName)
                status = MembershipCreateStatus.ProviderError;

            return null;
        }

        /// <summary>
        /// Updates an existing user with new settings
        /// </summary>
        /// <param name="user">MembershipUser object to modify</param>
        public override void UpdateUser(MembershipUser user)
        {
            Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
            MemberInfo m = dal.GetByName(user.UserName).SingleOrDefault();
            if (m != null)
            {
                m.Email = user.Email;
                m.IsValid = user.IsApproved;
                m.LastVisitDate = DateTime.UtcNow;                
            }
            //todo:
            dal.UpdateVisit(m);
        }

        /// <summary>
        /// Unlocks a user (after too many login attempts perhaps)
        /// </summary>
        /// <param name="userName">Username to unlock</param>
        /// <returns>True if successful. Defaults to false.</returns>
        public override bool UnlockUser(string userName)
        {
            // Return status defaults to false
            bool ret;
            try
            {
                    Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                    MemberInfo m = dal.GetByName(userName).SingleOrDefault();
                    m.Status = 1;
                    dal.Update(m);
                // A user was found and nothing was thrown
                ret = true;
            }
            catch
            {
                // Couldn't find the user or there was an error
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Permanently deletes a user from the database
        /// </summary>
        /// <param name="username">Username to delete</param>
        /// <param name="deleteAllRelatedData">Should or shouldn't delete related user data</param>
        /// <returns>True if successful. Defaults to false.</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            bool ret;

            try
            {
                Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                MemberInfo m = dal.GetByName(username).SingleOrDefault();
                dal.Delete(m);

                IRoles roleDal = Factory<IRoles>.Create("Role");
                roleDal.RemoveUsersFromRoles(new[] {username}, roleDal.GetRolesForUser(username));

                    
                if (deleteAllRelatedData)
                {
                    IMember membershipDal = Factory<IMember>.Create("Member");
                    membershipDal.DeleteProfile(m);
                }
                // Nothing was thrown, so go ahead and return true
                ret = true;
            }
            catch
            {
                // Couldn't find the user or was not able to delete
                ret = false;
            }

            return ret;
        }

        /*************************************************************************
         * User authentication methods
         *************************************************************************/

        /// <summary>
        /// Authenticates a user with the given username and password
        /// </summary>
        /// <param name="password">The login username</param>
        /// <param name="username">Login password</param>
        /// <returns>True if successful. Defaults to false.</returns>
        public override bool ValidateUser(string username, string password)
        {
            // Return status defaults to false.
            bool ret = false;

            try
            {
                    Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                    MemberInfo m = dal.GetByName(username).SingleOrDefault();

                    // We found a user by the username
                    if (m != null)
                    {
                        // A user cannot login if not approved or locked out
                        if ((!m.IsValid) || m.Status == 0)
                        {
                        }
                        else
                        {
                            // Trigger period
                            DateTime dt = DateTime.UtcNow;

                            // Check the given password and the one stored (and salt, if it exists)
                            if (CheckPassword(password, m.Password, ""))
                            {
                                m.LastVisitDate = dt; 
                                m.LastUpdateDate = dt;
                                // Save changes
                                dal.UpdateVisit(m);
                                // Reset past failures
                                //ResetAuthenticationFailures(ref m, dt);

                                ret = true;
                            }

                        }


                    }
            }
            catch (Exception)
            {
                // Nothing was thrown, so go ahead and return true
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Gets the current password of a user (provided it isn't hashed)
        /// </summary>
        /// <param name="username">User the password is being retrieved for</param>
        /// <param name="answer">Password retrieval answer</param>
        /// <returns>User's passsword</returns>
        public override string GetPassword(string username, string answer)
        {
            // Default password is empty
            string password = String.Empty;

            if (PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                throw new ProviderException("Hashed passwords cannot be retrieved. They must be reset.");
            }
            return password;
        }

        /// <summary>
        /// Resets the passwords with a generated value
        /// </summary>
        /// <param name="username">User the password is being reset for</param>
        /// <param name="answer">Password retrieval answer</param>
        /// <returns>Newly generated password</returns>
        public override string ResetPassword(string username, string answer)
        {
            // Default password is empty
            string pass = String.Empty;

            try
            {
                    Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                    MemberInfo m = dal.GetByName(username).SingleOrDefault();

                    // We found a user by that name
                    if (m != null)
                    {
                        // Check if the returned password answer matches
                        if (_requiresQuestionAndAnswer)
                        {
                            if (EncodePassword(answer) == m.ValidationKey)
                            {
                                // Create a new password with the minimum number of characters
                                pass = GeneratePassword(MinRequiredPasswordLength);

                                // If the password format is hashed, there must be a salt added
                                if (PasswordFormat == MembershipPasswordFormat.Hashed)
                                {
                                    //string salt = GenerateSalt();
                                    //pass = pass + salt;
                                    m.Password = SHA256Hash(pass);
                                    dal.Update(m);
                                }

                                //m.Password = EncodePassword(pass);
                                //m.PasswordSalt = salt;

                                // Reset everyting
                                //ResetAuthenticationFailures(ref m, DateTime.UtcNow);

                                
                            }
                        }else
                        {
                            if (PasswordFormat == MembershipPasswordFormat.Hashed)
                            {
                                //string salt = GenerateSalt();
                                //pass = pass + salt;
                                pass = GeneratePassword(MinRequiredPasswordLength);
                                m.Password = SHA256Hash(pass);
                                dal.Update(m);
                            }
                        }
                    }
            }
            catch
            {
            }
            return pass;
        }

        /// <summary>
        /// Change the current password for a new one. Note: Both are required.
        /// </summary>
        /// <param name="username">Username the password is being changed for</param>
        /// <param name="oldPassword">Old password to verify owner</param>
        /// <param name="newPassword">New password</param>
        /// <returns>True if successful. Defaults to false.</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (!ValidateUser(username, oldPassword))
                return false;

            var args = new ValidatePasswordEventArgs(username, newPassword, false);

            OnValidatingPassword(args);

            if (args.Cancel)
                if (args.FailureInformation != null)
                    throw args.FailureInformation;
                else
                    throw new MembershipPasswordException("Password change has been cancelled due to a validation failure.");

            bool ret;
            try
            {
                    Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                    MemberInfo m = dal.GetByName(username).SingleOrDefault();

                    if (PasswordFormat == MembershipPasswordFormat.Hashed)
                    {
                        //string salt = GenerateSalt();
                        //newPassword = newPassword + salt;
                        m.Password = SHA256Hash(newPassword);
                    }

                    dal.Update(m);
                ret = true;
            }
            catch
            {
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// Change the password retreival/reset question and answer pair
        /// </summary>
        /// <param name="username">Username the question and answer are being changed for</param>
        /// <param name="password">Current password</param>
        /// <param name="newPasswordQuestion">New password question</param>
        /// <param name="newPasswordAnswer">New password answer (will also be encrypted)</param>
        /// <returns>True if successful. Defaults to false.</returns>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password,
            string newPasswordQuestion, string newPasswordAnswer)
        {
            if (newPasswordAnswer != "validationcode")
            if (!ValidateUser(username, password))
                return false;

            bool ret;
            try
            {
                    Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                    MemberInfo m = dal.GetByName(username).SingleOrDefault();

                    m.ValidationKey = newPasswordQuestion;
                    m.PasswordChangeKey = newPasswordAnswer;

                    dal.Update(m);
                ret = true;
            }
            catch
            {
                ret = false;
            }
            return ret;
        }


        /*************************************************************************
         * User information retreival methods
         *************************************************************************/

        /// <summary>
        /// Gets the username by a given matching email address
        /// </summary>
        public override string GetUserNameByEmail(string email)
        {
            string username = String.Empty;

            try
            {
                Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                MemberInfo m = dal.GetByEmail(email);
            }
            catch
            {
            }
            return username;
        }

        /// <summary>
        /// Gets a MembershipUser object with a given key
        /// </summary>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            SnitzMembershipUser u = null;
            try
            {
                Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                MemberInfo m = dal.GetById(Convert.ToInt32(providerUserKey));

                if (userIsOnline)
                {

                    m.LastUpdateDate = DateTime.UtcNow;
                    dal.UpdateVisit(m);
                }
                if (m != null)
                    u = GetUserFromMember(m);
            }
            catch
            { }

            return u;
        }

        /// <summary>
        /// Gets a MembershipUser object with a given username
        /// </summary>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SnitzMembershipUser u = null;

            try
            {
                    Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                    MemberInfo m = dal.GetByName(username).SingleOrDefault();

                    if (userIsOnline)
                    {
                        m.LastUpdateDate = DateTime.UtcNow;
                        dal.UpdateVisit(m);
                    }
                    if (m != null)
                        u = GetUserFromMember(m);
            }
            catch
            { }

            return u;
        }

        /// <summary>
        /// Gets all the users in the database
        /// </summary>
        /// <param name="pageIndex">Current page index</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="totalRecords">Total number of users returned</param>
        /// <returns>MembershpUserCollection object with a list of users on the page</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize,
            out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                int start = pageSize * pageIndex;
                
                totalRecords = GetMemberCount();
                Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                List<MemberInfo> mlist = new List<MemberInfo>(dal.GetMembers(pageIndex * pageSize, pageSize, null, null));

                foreach (MemberInfo m in mlist)
                {
                    SnitzMembershipUser mu = GetUserFromMember(m);
                    users.Add(mu);
                }

            }
            catch
            {
            }

            return users;
        }

        /// <summary>
        /// Gets the total number of users that are currently online.
        /// </summary>
        /// <returns>Returns user count (within UserIsOnlineTimeWindow minutes)</returns>
        public override int GetNumberOfUsersOnline()
        {
            int c;
            try
            {
                IMember membershipDal = Factory<IMember>.Create("Member");

                c = membershipDal.OnlineUsers(UserIsOnlineTimeWindow).Count();
            }finally
            {
            }
            //catch { }

            return c;
        }

        /// <summary>
        /// Finds a list of users with a matching email address
        /// </summary>
        /// <param name="emailToMatch">Given email to search</param>
        /// <param name="pageIndex">Current page index</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="totalRecords">Total number of users returned</param>
        /// <returns>MembershpUserCollection object with a list of users on the page</returns>
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch,
            int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;



            return users;
        }

        /// <summary>
        /// Gets a list of users with a matching username
        /// </summary>
        /// <param name="usernameToMatch">Username to search for</param>
        /// <param name="pageIndex">Current page index</param>
        /// <param name="pageSize">Number of results per page</param>
        /// <param name="totalRecords">Total number of users returned</param>
        /// <returns>MembershpUserCollection object with a list of users on the page</returns>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch,
            int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                int start = pageSize * pageIndex;
                Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                List<MemberInfo> mlist = new List<MemberInfo>(dal.GetByName(usernameToMatch));

                foreach (MemberInfo m in mlist)
                        users.Add(GetUserFromMember(m));
            }
            catch { }

            return users;
        }

        public MembershipUserCollection FindUsersByInitial(string initial,
            int pageIndex, int pageSize, out int totalRecords)
        {
            var users = new MembershipUserCollection();
            totalRecords = 0;

            try
            {
                int start = pageSize * pageIndex;
                Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                List<MemberInfo> mlist = new List<MemberInfo>(dal.GetByName(initial + "%"));

                foreach (MemberInfo m in mlist)
                    users.Add(GetUserFromMember(m));
            }
            catch { }

            return users;
        }
        /*************************************************************************
         * Class initialization
         *************************************************************************/

        public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
        {
            if (config == null)
                throw new ArgumentNullException("config");


            if (String.IsNullOrEmpty(name))
                name = "SnitzMembershipProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Snitz Membership Provider");
            }

            // Initialize base class
            base.Initialize(name, config);

            _applicationName = GetConfigValue(config["applicationName"],
                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

            // This is a non-standard helper setting.
            ProviderName = GetConfigValue(config["providerName"], name);


            // Sets the default parameters for all the Membership Provider settings

            _requiresUniqeEmail = Config.UniqueEmail; // Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"));
            _requiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"));
            _minRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "5"));
            _minRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"],"0"));
            _enablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"));
            _enablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "false"));
            _passwordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"));
            _passwordStrengthRegularExpression = GetConfigValue(config["passwordStrengthRegularExpression"], "");
            _maxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"],"5"));

            string passFormat = config["passwordFormat"] ?? "hashed";

            // If no format is specified, the default format will be hashed.

            switch (passFormat.ToLower())
            {
                case "hashed":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "encrypted":
                    _passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                case "clear":
                    _passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                default:
                    throw new ProviderException("Password format '" + passFormat + "' is not supported. Check your web.config file.");
            }

            Configuration cfg = WebConfigurationManager.OpenWebConfiguration(
                System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);

            _machineKey = (MachineKeySection)cfg.GetSection("system.web/machineKey");

            //if (machineKey.ValidationKey.Contains("AutoGenerate"))
            //    if (PasswordFormat != MembershipPasswordFormat.Clear)
            //        throw new ProviderException("Hashed or Encrypted passwords cannot be used with auto-generated keys.");

            var membership = (MembershipSection)cfg.GetSection("system.web/membership");
            _userIsOnlineTimeWindow = membership.UserIsOnlineTimeWindow;
        }


        /*************************************************************************
         * Private password helper methods
         *************************************************************************/

        /// <summary>
        /// Compares a given password with one stored in the database and an optional salt
        /// </summary>
        private bool CheckPassword(string password, string dbpassword, string dbsalt)
        {
            string pass1 = password;
            string pass2 = dbpassword;
            bool ret = false;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Encrypted:
                    pass2 = UnEncodePassword(dbpassword);
                    break;
                case MembershipPasswordFormat.Hashed:
                    pass1 = SHA256Hash(password);
                    break;
                default:
                    break;
            }

            if (pass1 == pass2)
                ret = true;

            return ret;
        }

        /// <summary>
        /// Encodes a given password using the default MembershipPasswordFormat setting
        /// </summary>
        /// <param name="password">Password (plus salt as per above functions if necessary)</param>
        /// <returns>Clear form, Encrypted or Hashed password.</returns>
        private string EncodePassword(string password)
        {
            string encodedPassword = password;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    encodedPassword =
                      Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    encodedPassword = SHA256Hash(password);
                    break;
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return encodedPassword;
        }

        /// <summary>
        /// Decodes a given stored password into a cleartype or unencrypted form. Provided it isn't hashed.
        /// </summary>
        /// <param name="encodedPassword">Stored, encrypted password</param>
        /// <returns>Unecncrypted password</returns>
        private string UnEncodePassword(string encodedPassword)
        {
            string password = encodedPassword;

            switch (PasswordFormat)
            {
                case MembershipPasswordFormat.Clear:
                    break;
                case MembershipPasswordFormat.Encrypted:
                    password =
                      Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
                    break;
                case MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Cannot decode hashed passwords.");
                default:
                    throw new ProviderException("Unsupported password format.");
            }

            return password;
        }

        /// <summary>
        /// Converts a string into a byte array
        /// </summary>
        private byte[] HexToByte(string hexString)
        {
            var returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// Salt generation helper (this is essentially the same as the one in SqlMembershipProviders
        /// </summary>
        private string GenerateSalt()
        {
            var buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// Generates a random password of given length (MinRequiredPasswordLength)
        /// </summary>
        public string GeneratePassword(int passLength)
        {
            const string range = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var bytes = new Byte[passLength];
            var chars = new char[passLength];

            var rng = new RNGCryptoServiceProvider();

            rng.GetBytes(bytes);

            for (int i = 0; i < passLength; i++)
                chars[i] = range[bytes[i] % range.Length];

            return new string(chars);
        }
        
        private string SHA256Hash(string data)
        {
            SHA256 sha = new SHA256Managed();
            byte[] hash = sha.ComputeHash(Encoding.ASCII.GetBytes(data));

            var stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }
        /*************************************************************************
         * Private helper methods
         *************************************************************************/

        /// <summary>
        /// Used in the initializtion, key in web.config or the default setting if null.
        /// </summary>
        private string GetConfigValue(string configValue, string defaultValue)
        {
            if (String.IsNullOrEmpty(configValue))
                return defaultValue;

            return configValue;
        }

        /// <summary>
        /// Converts a Member object into a MembershipUser object using its assigned settings
        /// </summary>
        private SnitzMembershipUser GetUserFromMember(MemberInfo m)
        {
            var smu = new SnitzMembershipUser(this.ProviderName,
                        m.Username,
                        m.Id,
                        m.Email,
                        m.ValidationKey,
                        "",
                        m.IsValid,
                        m.Status == 0,
                        m.MemberSince,
                        m.LastVisitDate == null ? DateTime.MinValue : m.LastVisitDate.Value,
                        m.LastVisitDate == null ? DateTime.MinValue : m.LastVisitDate.Value,  //m.LastUpdateDate.Value,
                        DateTime.MinValue,
                        DateTime.MinValue,
                        m.LastPostDate,
                        m.Title,
                        m.Country,
                        m.PostCount);
            
            return smu;
        }

        /*************************************************************************
         * Forum helper methods
         *************************************************************************/

        public static bool ActivateUser(string username)
        {
            IMember dal = Factory<IMember>.Create("Member");
            return dal.ActivateUser(username);
        }
        public static bool ChangeEmail(MembershipUser user, bool valid, string email)
        {
            IMember dal = Factory<IMember>.Create("Member");
            return dal.ChangeEmail(user.UserName, valid, email);
        }
        public static string CreateValidationCode(MembershipUser user)
        {
            byte[] userNameBytes = Encoding.UTF32.GetBytes(user.UserName);
            byte[] emailBytes = Encoding.UTF32.GetBytes(user.Email);
            byte[] createDateBytes = Encoding.UTF32.GetBytes(user.CreationDate.ToString());

            int validationcode = 0;
            foreach (byte value in userNameBytes) { validationcode += value; }
            foreach (byte value in emailBytes) { validationcode += value; }
            foreach (byte value in createDateBytes) { validationcode += value; }

            validationcode -= (user.UserName.Length + user.Email.Length + user.CreationDate.ToString().Length);
            return validationcode.ToString();

        }
        public bool CheckValidationCode(MembershipUser user, string validationcode)
        {
            return CreateValidationCode(user) == validationcode;
        }

        public static int GetUnApprovedMemberCount()
        {
            IMember dal = Factory<IMember>.Create("Member");
            return dal.UnApprovedMemberCount();
        }

        public static int GetMemberCount()
        {
            IMember dal = Factory<IMember>.Create("Member");
            return dal.MemberCount();

        }

        public string[] GetOnlineUsers()
        {
            IMember dal = Factory<IMember>.Create("Member");
            return dal.OnlineUsers(UserIsOnlineTimeWindow);
        }
        public bool LockUser(string username)
        {
            // Return status defaults to false
            bool ret;
            try
            {
                Snitz.IDAL.IMember dal = Snitz.IDAL.Factory<IDAL.IMember>.Create("Member");
                MemberInfo m = dal.GetByName(username).SingleOrDefault();
                m.Status = 0;
                dal.Update(m);
                // A user was found and nothing was thrown
                ret = true;
            }
            catch
            {
                // Couldn't find the user or there was an error
                ret = false;
            }

            return ret;
        }
    }
}
