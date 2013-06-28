using System;
using System.Linq;
using SnitzCommon;

namespace SnitzUI.UserControls
{
    public partial class MiniActiveTopics : System.Web.UI.UserControl
    {
        protected PageBase ThisPage;
        public int TopicCount { get; set; }
        public bool Hide { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            ThisPage = (PageBase)Page;
            var activetopics = SnitzData.PagedObjects.GetActiveTopics(TopicCount);
            if (!activetopics.Any())
            {
                Hide = true;
            }
            else
            {
                Hide = false;
                DataList1.DataSource = activetopics;
                DataList1.DataBind();
            }
        }


    }
}