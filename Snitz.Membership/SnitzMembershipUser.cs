using System;
using System.ComponentModel;
using System.Web;
using System.Web.Profile;
using System.Web.Security;


/// <summary>
/// Summary description for SnitzMembershipUser
/// </summary>
[Serializable]
public class SnitzMembershipUser : MembershipUser
{
    private readonly MembershipUser mu;
    private string _sig;

    public string Sig
    {
        get
        {
            return _sig ?? String.Empty;
            //try
            //{
            //    ProfileBase p = new ProfileBase();
            //    string test = p.UserName;

            //    //Prof _profile = new ProfileCommon().GetProfile(username);
            //    return HttpContext.Current.Profile.GetPropertyValue("Sig").ToString();
            //}catch
            //{
            //    return String.Empty;
            //}
        }
        set
        {
            _sig = value;
        }
    }

    public MembershipUser Mu
    {
        get { return mu; }
    }

    public bool IsActive
    {
        get { return LastActivityDate > DateTime.UtcNow.AddMinutes(-Membership.UserIsOnlineTimeWindow); }
    }

    public string Title { get; private set; }
    public DateTime? LastPostDate { get; set; }
    public string Country { get; set; }

    public int Posts { get; set; }

    public SnitzMembershipUser()
    {

	}
    [DataObjectField(true)]
    public override string UserName
    {
        get { return base.UserName; }
    }
    public override DateTime LastLoginDate
    {
        get
        {
            return base.LastLoginDate;
        }
        set
        {
            base.LastLoginDate = value;
        }
    }
    public SnitzMembershipUser(MembershipUser mu)
    {
        this.mu = mu;
    }
    public SnitzMembershipUser (MembershipUser mu, int posts,string country)
    {
        this.mu = mu;
        Posts = posts;
        Title = mu.Comment;
        Country = country;
    }
    public SnitzMembershipUser(string providername,
                                  string username,
                                  object providerUserKey,
                                  string email,
                                  string passwordQuestion,
                                  string comment,
                                  bool isApproved,
                                  bool isLockedOut,
                                  DateTime creationDate,
                                  DateTime lastLoginDate,
                                  DateTime lastActivityDate,
                                  DateTime lastPasswordChangedDate,
                                  DateTime lastLockedOutDate,
                                  DateTime? lastPostDate,
                                  string title,
                                  string country,
                                  int posts) : base( providername,username,providerUserKey,email,passwordQuestion,comment,isApproved,isLockedOut,creationDate,lastLoginDate,lastActivityDate,lastPasswordChangedDate,lastLockedOutDate)
    {
        LastPostDate = lastPostDate;
            Posts = posts;
            Country = country;
            Title = title;
        }

}
