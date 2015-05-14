/*
####################################################################################################################
##
## SnitzMembership - SnitzMembershipUser
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
using System.ComponentModel;
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
        get
        {
            return LastActivityDate > DateTime.UtcNow.AddMinutes(-Membership.UserIsOnlineTimeWindow);
        }
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
        base.LastActivityDate = lastActivityDate;
        LastPostDate = lastPostDate;
            Posts = posts;
            Country = country;
            Title = title;
        }

}
