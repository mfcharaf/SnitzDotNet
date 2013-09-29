/*
####################################################################################################################
##
## SnitzMembership - ProfileCommon
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
using System.Configuration;
using System.Web.Profile;
using System.Web.Security;
using System.Web;
using Snitz.Entities;

namespace SnitzMembership
{
    public class ProfileCommon : ProfileBase
    {

        public static ProfileCommon GetProfile()
        {
            return Create(HttpContext.Current.Request.IsAuthenticated ?
                   HttpContext.Current.User.Identity.Name : HttpContext.Current.Request.AnonymousID,
                   HttpContext.Current.Request.IsAuthenticated) as ProfileCommon;
        }

        public static ProfileCommon GetUserProfile(string username)
        {
            return Create(username) as ProfileCommon;
        }

        public static ProfileCommon GetUserProfile()
        {
            return Create(Membership.GetUser().UserName) as ProfileCommon;
        }

        // Add the Profile properties starting here
        // using the same property name and SQL type
        // as exists in your new profile table


        [CustomProviderData("Skype;nvarchar")]
        public virtual string Skype
        {
            get
            {
                try
                {
                    return ((string)(this.GetPropertyValue("Skype")));
                }
                catch (SettingsPropertyNotFoundException)
                {
                    return string.Empty;
                }
                
            }
            set
            {
                this.SetPropertyValue("Skype", value);
            }
        }

        [CustomProviderData("LinkTarget;nvarchar")]
        public virtual string LinkTarget
        {
            get
            {
                try
                {
                    return ((string) (this.GetPropertyValue("LinkTarget")));
                }catch(SettingsPropertyNotFoundException)
                {
                    return string.Empty;
                }
            }
            set
            {
                this.SetPropertyValue("LinkTarget", value);
            }
        }

        [CustomProviderData("HideAge;int")]
        public virtual bool HideAge
        {
            get
            {
                try
                {
                    object age = this.GetPropertyValue("HideAge");
                    if (age != null)
                        return (Convert.ToInt16(age) == 1);
                }
                catch (SettingsPropertyNotFoundException)
                {
                }
                return false;
            }
            set
            {
                this.SetPropertyValue("HideAge", value);
            }
        }

        [CustomProviderData("Gravatar;int")]
        public virtual bool Gravatar
        {
            get
            {
                try
                {
                    object gravatar = this.GetPropertyValue("Gravatar");
                    if (gravatar != null)
                        return (Convert.ToInt16(gravatar) == 1);
                    return false;
                }
                catch (SettingsPropertyNotFoundException)
                {
                    
                    return false;
                }
            }
            set
            {
                this.SetPropertyValue("Gravatar",value);
            }
        }

        [CustomProviderData("FavLinks;nvarchar")]
        public virtual List<SnitzLink> FavLinks
        {
            get
            {
                try
                {
                    List<SnitzLink> links = (List<SnitzLink>) this.GetPropertyValue("FavLinks");
                    return links;
                }catch(SettingsPropertyNotFoundException )
                {
                    return new List<SnitzLink>();
                }
            }
            set
            {
                this.SetPropertyValue("FavLinks", value);
            }
        }

        [CustomProviderData("BookMarks;nvarchar")]
        public virtual List<SnitzLink> BookMarks
        {
            get
            {
                try
                {
                    List<SnitzLink> links = (List<SnitzLink>) this.GetPropertyValue("BookMarks");
                    return links;
                }catch(SettingsPropertyNotFoundException)
                {
                    return new List<SnitzLink>();
                }
            }
            set
            {
                this.SetPropertyValue("BookMarks", value);
            }
        }

        [CustomProviderData("PublicGallery;int")]
        public virtual bool PublicGallery
        {
            get
            {
                try
                {
                    object gallery = this.GetPropertyValue("PublicGallery");
                    if (gallery != null)
                        return (Convert.ToInt16(gallery) == 1);
                    return false;
                }
                catch (SettingsPropertyNotFoundException)
                {

                    return false;
                }
            }
            set
            {
                this.SetPropertyValue("PublicGallery", value);
            }
        }
        
        [CustomProviderData("TimeOffset;int")]
        public virtual int TimeOffset
        {
            get
            {
                try
                {
                    object timeoffset = this.GetPropertyValue("TimeOffset");
                    if (timeoffset != null)
                        return Convert.ToInt32(timeoffset);
                    return 0;
                }
                catch (SettingsPropertyNotFoundException)
                {

                    return 0;
                }
            }
            set
            {
                this.SetPropertyValue("TimeOffset", value);
            }
        }

        [CustomProviderData("PMEmail;int")]
        public virtual int? PMEmail
        {
            get
            {
                try
                {
                    object pmemail = this.GetPropertyValue("PMEmail");
                    if (pmemail != null)
                        return Convert.ToInt32(pmemail);
                    return 0;
                }
                catch (SettingsPropertyNotFoundException)
                {

                    return 0;
                }
            }
            set
            {
                this.SetPropertyValue("PMEmail", value);
            }
        }

        [CustomProviderData("PMReceive;int")]
        public virtual int? PMReceive
        {
            get
            {
                try
                {
                    object pmreceive = this.GetPropertyValue("PMReceive");
                    if (pmreceive != null)
                        return Convert.ToInt32(pmreceive);
                    return 0;
                }
                catch (SettingsPropertyNotFoundException)
                {

                    return 0;
                }
            }
            set
            {
                this.SetPropertyValue("PMReceive", value);
            }
        }

        [CustomProviderData("PMLayout;nvarchar")]
        public virtual string PMLayout
        {
            get
            {
                try
                {
                    object pmlayout = this.GetPropertyValue("PMLayout");
                    return (string) pmlayout;
                }
                catch (SettingsPropertyNotFoundException)
                {

                    return null;
                }
            }
            set
            {
                this.SetPropertyValue("PMLayout", value);
            }
        }
    }

}