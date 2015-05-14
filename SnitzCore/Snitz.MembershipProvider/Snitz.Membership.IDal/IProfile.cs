using System;
using System.Configuration;
using System.Web.Profile;

namespace Snitz.Membership.IDal
{
    public interface IProfile
    {
        string TableName { get; set; }
        SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection collection);
        void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection collection);
        int DeleteProfiles(ProfileInfoCollection profiles);
        int DeleteProfiles(string[] usernames);
        string GetQueryFromCommand(object cmd);
        int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);
        int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate);

        ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex,int pageSize, out int totalRecords);
        ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords);

        ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption,
            string usernameToMatch, int pageIndex, int pageSize, out int totalRecords);

        ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption,
            string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords);

        string GetBookMarkModValues(string username);
    }
}
