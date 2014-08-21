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


        [CustomProviderData("Skype;varchar")]
        public virtual string Skype
        {
            get
            {
                try
                {
                    var value = this.GetPropertyValue("Skype");
                    if (value is DBNull)
                        return null;
                    else
                        return ((string)(value));
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

        [CustomProviderData("LinkTarget;varchar")]
        public virtual string LinkTarget
        {
            get
            {
                try
                {
                    var value = this.GetPropertyValue("LinkTarget");
                    if (value is DBNull)
                        return null;
                    return ((string)(value));
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

        [CustomProviderData("HideAge;smallint")]
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

        [CustomProviderData("Gravatar;smallint")]
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

        [CustomProviderData("FavLinks;varchar")]
        public virtual List<SnitzLink> FavLinks
        {
            get
            {
                try
                {
                    var value = this.GetPropertyValue("FavLinks");
                    if (value is DBNull)
                        return null;
                    List<SnitzLink> links = (List<SnitzLink>) value;
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

        [CustomProviderData("BookMarks;varchar")]
        public virtual List<SnitzLink> BookMarks
        {
            get
            {
                try
                {
                    var value = this.GetPropertyValue("BookMarks");
                    if (value is DBNull)
                        return null;
                    List<SnitzLink> links = (List<SnitzLink>)value;
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

        [CustomProviderData("PublicGallery;smallint")]
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
        
        [CustomProviderData("PMEmail;smallint")]
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

        [CustomProviderData("PMReceive;smallint")]
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

        [CustomProviderData("PMLayout;varchar")]
        public virtual string PMLayout
        {
            get
            {
                try
                {
                    object pmlayout = this.GetPropertyValue("PMLayout");
                    if (pmlayout is DBNull)
                        return null;
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