using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Profile;
using System.Web.Security;
using System.Web;
using SnitzCommon;

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
    }

}