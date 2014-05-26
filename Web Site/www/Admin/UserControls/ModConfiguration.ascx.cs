using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.UI.WebControls;
using ModConfig;
using SnitzCommon;
using SnitzConfig;
using SnitzUI.Admin;


public partial class Admin_ModConfiguration : System.Web.UI.UserControl
{   
    Dictionary<string, string> modProps = new Dictionary<string, string>();

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!Page.IsPostBack)
        {
            PopulateList(ConfigHelper.GetModConfigs());
        }

    }

    private void PopulateList(IEnumerable<string> modconfigs)
    {
        foreach (string modconfig in modconfigs)
        {
            Type target = Type.GetType("ModConfig." + modconfig + ",App_Code", true);
            var modDisplay = ConfigHelper.ModDisplayName(modconfig);
            
            PropertyInfo[] properties = target.GetProperties(BindingFlags.Public | BindingFlags.Static);
            modProps.Add("<b>" + modDisplay + "</b>", "ModConfig." + modconfig);
            foreach (PropertyInfo prop in properties)
            {
                modProps.Add(prop.Name, prop.GetValue(target, null).ToString());

            }
        }

        DataList1.DataSource = modProps;
        DataList1.DataBind();
        
    }

    protected void B1_Click(object sender, EventArgs e)
    {
        

        //Save the config values
        var toUpdate = new Dictionary<string, string>();
        object modInstance = null;
        for (int i = 0; i < DataList1.Items.Count; i++)
        {
            Panel ctrl = (Panel)DataList1.Items[i].FindControl("ctrl");
            if (ctrl != null)
            {
                Label key = (Label)DataList1.Items[i].FindControl("Label1");
                TextBox editor = (TextBox)DataList1.Items[i].FindControl("txtCtrl");
                AdminRadioButton radio = (AdminRadioButton)DataList1.Items[i].FindControl("rblCtrl");
                
                if (!editor.Text.StartsWith("ModConfig."))
                {
                    if (editor.Visible)
                    {
                        toUpdate.Add(key.Text.GetModPropertyDescription(modInstance), editor.Text);
                    }
                    else
                        toUpdate.Add(key.Text.GetModPropertyDescription(modInstance), radio.SelectedValue);
                }
                else
                {
                    CheckBox enable = (CheckBox)DataList1.Items[i].FindControl("chkEnable");
                    Type target = Type.GetType(editor.Text + ",App_Code", true);
                    modInstance = Activator.CreateInstance(target);

                    toUpdate.Add("Enabled".GetModPropertyDescription(modInstance),enable.Checked ? "1" : "0");
                }
                
            }
        }
        Config.UpdateKeys(toUpdate);
    }

    protected void DataList1_ItemDataBound(object sender, DataListItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item ||
             e.Item.ItemType == ListItemType.AlternatingItem)
        {
            Panel ctrl = (Panel) e.Item.FindControl("ctrl");
            Panel ctrlHead = (Panel)e.Item.FindControl("ctrlHead");
            TextBox editor = (TextBox)e.Item.FindControl("txtCtrl");
            AdminRadioButton radio = (AdminRadioButton)e.Item.FindControl("rblCtrl");
            Label lbl = (Label)e.Item.FindControl("Label2");
            if(editor != null){
                KeyValuePair<string, string> mod = (KeyValuePair<string, string>)e.Item.DataItem;
                if (mod.Value.StartsWith("ModConfig."))
                {
                    var modconfig = mod.Value.Replace("ModConfig.", "");
                    CheckBox enabled = (CheckBox) e.Item.FindControl("chkEnable");
                    if (enabled != null)
                    {
                        enabled.Checked = ConfigHelper.IsModEnabled(modconfig);
                    }
                    ctrl.Visible = false;
                    ctrlHead.Visible = true;
                    lbl.Visible = true;
                    lbl.Text = ConfigHelper.ModDisplayName(modconfig) + " v" + ConfigHelper.ModVersion(modconfig);
                    ctrlHead.CssClass = "forumtable";
                }
                else
                {
                    ctrl.Visible = true;
                    ctrlHead.Visible = false;
                    lbl.Visible = false;
                    if (mod.Value == "True" || mod.Value == "False")
                    {
                        editor.Visible = false;
                        radio.Visible = true;
                        radio.SelectedValue = mod.Value == "True" ? "1" : "0";
                    }
                    else
                    {
                        radio.Visible = false;
                        editor.Visible = true;
                    }
                }
            }
        }
    }
}
