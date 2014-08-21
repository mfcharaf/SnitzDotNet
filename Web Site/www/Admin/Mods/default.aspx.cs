using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using ModConfig;
using Snitz.BLL.modconfig;
using SnitzCommon;

namespace SnitzUI.Admin.Mods
{
    public partial class _default : PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            pageCSS.Attributes.Add("href", "/Admin/css/" + Page.Theme + "/admin.css");
            menuCSS.Attributes.Add("href", "/Admin/css/" + Page.Theme + "/menu.css");
            PopulateList(ConfigHelper.GetModConfigs());
            bool? addBPOP = ViewState["AddBPOP"] as bool?;
            if (addBPOP.HasValue && addBPOP.Value)
            {
                AddModControl(ViewState["AddBPOPmodname"].ToString());
            }

        }

        private void PopulateList(List<string> mods)
        {
            Dictionary<string, string> modProps = new Dictionary<string, string>();

            foreach (string mod in mods)
            {
                string modname = ConfigHelper.ModDisplayName(mod);
                
                modProps.Add("<b>" + modname + "</b>", mod);
            }
            ModMenu.DataSource = modProps;
            ModMenu.DataBind();
        }

        protected void LoadMod(object sender, EventArgs e)
        {

            string modname = ((LinkButton)sender).CommandArgument;
            AddModControl(modname);
            ViewState["AddBPOP"] = true;
            ViewState["AddBPOPmodname"] = modname;

        }

        private void AddModControl(string modname)
        {
            modPh.Controls.Clear();
            
            ModConfigBase controller = (ModConfigBase)ConfigHelper.ModClass(modname);
            if (!String.IsNullOrEmpty(controller.ModConfiguration.AdminControl))
            {
                var adminSys = Page.LoadControl(controller.ModConfiguration.AdminControl);
                adminSys.ID = "modPnl";
                modPh.Controls.Add(adminSys);
            }
            else
            {
                DefaultMod adminSys = (DefaultMod)Page.LoadControl("DefaultMod.ascx");
                adminSys.ID = "modPnl";
                adminSys.ModName = controller.Name;
                modPh.Controls.Add(adminSys);
            }
        }
    }
}