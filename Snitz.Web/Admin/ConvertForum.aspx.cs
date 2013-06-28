using System;
using System.Web.Profile;
using System.Web.UI;

public partial class ConvertForum : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {

        ProfileInfoCollection pcol = ProfileManager.GetAllProfiles(ProfileAuthenticationOption.All);

        foreach (ProfileInfo pI in pcol)
        {
            ProfileCommon p = Profile.GetProfile(pI.UserName);
            if (p.Sig.Trim() != "")
            {
                p.Sig = Import.CleanCode(p.Sig);
                p.Save();
            }
        }
        
     }
}
