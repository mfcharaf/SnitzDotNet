using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzData;


public partial class Admin_ManageAvatars : UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
        {
            PopulateList();
        }
    }

    private void PopulateList()
    {
        Avatars.DataSource = Avatar.GetAvatars();
        Avatars.DataBind();
    }

    protected void Avatars_ItemCommand(object source, DataListCommandEventArgs e)
    {
        switch(e.CommandName)
        {
            case "select":
                Avatars.SelectedIndex = e.Item.ItemIndex;
                break;
            case "delete":
                Avatar.Delete(e.CommandArgument.ToString());
                break;
        }
        PopulateList();
    }
}
