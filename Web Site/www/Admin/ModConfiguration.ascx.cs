using System;
using System.Collections.Generic;
using System.Data;

using System.Reflection;
using System.Web.UI.WebControls;


public partial class Admin_ModConfiguration : System.Web.UI.UserControl
{   
    Dictionary<string, string> modProps = new Dictionary<string, string>();

    protected void Page_Load(object sender, EventArgs e)
    {

        List<string> mods = new List<string>(2);
        mods.Add("UploadConfig");
        mods.Add("SampleModConfig");

        if (!Page.IsPostBack)
        {
            PopulateList(mods);
        }

    }

    private void PopulateList(IEnumerable<string> mods)
    {
        foreach (string mod in mods)
        {
            Type target = Type.GetType(string.Format("{0},App_Code", "ModConfig." + mod), true);
            PropertyInfo pinfo = target.GetProperty("Description");
            pinfo.ToString();
            PropertyInfo[] properties = target.GetProperties(BindingFlags.Public | BindingFlags.Static);

            modProps.Add("<br/>" + mod,"XXXXXXXX");
            foreach (PropertyInfo prop in properties)
            {
                modProps.Add(prop.Name, prop.GetValue(target, null).ToString());

            }
        }
        List<string> test = SnitzCachedLists.GetAllClasses("ModConfig");

        DataList1.DataSource = modProps;
        DataList1.DataBind();
        
    }

    protected void B1_Click(object sender, EventArgs e)
    {
        //Save the config values
    }
    protected void DataList1_ItemDataBound(object sender, System.Web.UI.WebControls.DataListItemEventArgs e)
    {
        if (e.Item.ItemType == ListItemType.Item ||
             e.Item.ItemType == ListItemType.AlternatingItem)
        {
            TextBox editor = (TextBox)e.Item.FindControl("TextBox1");
            if(editor != null){
                KeyValuePair<string, string> mod = (KeyValuePair<string, string>)e.Item.DataItem;
                if (mod.Value == "XXXXXXXX")
                {
                    editor.Visible = false;
                    Label lbl = (Label) e.Item.FindControl("Label2");
                    lbl.Visible = true;
                    lbl.CssClass = "forumtable";
                }
            }
        }
    }
}
