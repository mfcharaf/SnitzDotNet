using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SnitzCommon.Controls
{
    public class AdminRegControl : Panel
    {
        private Label _lblField;
        public CheckBox CbxShow;
        public CheckBox CbxRequired;
        private string _text;
        private string _controlId;
        private bool _required;
        private bool _show;

        public virtual string LabelText
        {
            get { return _text; }
            set { _text = value; }
        }
        public virtual bool Required
        {
            get { return _required; }
            set { _required = value; }
        }
        public virtual bool Show
        {
            get { return _show; }
            set { _show = value; }
        }
        public virtual string ControlID
        {
            get { return _controlId; }
            set { _controlId = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            //Panel pnl = new Panel();
            CbxShow = new CheckBox() {Text="Show",CssClass = "cbxLabel", ID = _controlId + "Show" + this.ID, Checked = _show,Enabled = this.Enabled};
            CbxRequired = new CheckBox() { Text = "Required", CssClass = "cbxLabel", ID = _controlId + "Req" + this.ID, Checked = _required, Enabled = this.Enabled };

            _lblField = new Label { Text = _text, AssociatedControlID = CbxShow.ID };
            this.Controls.Add(_lblField);
            this.Controls.Add(CbxShow);
            this.Controls.Add(CbxRequired);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }
}