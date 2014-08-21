using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SnitzCommon.Controls
{
    public class RegControl : Panel
    {
        private Label _lblField;
        public Control CtrlField;
        private RequiredFieldValidator _rfvField;
        private string _text;
        private string _controlId;
        private string _control;
        private bool _required;
        private bool _show;
        private string _validationGroup;

        public virtual string ValidationGroup
        {
            get { return _validationGroup; }
            set { _validationGroup = value; }
        }
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

        public virtual string InvalidMessage { get; set; }

        public virtual string ClientScript { get; set; }

        public virtual string ControlID
        {
            get { return _controlId; }
            set { _controlId = value; }
        }

        public virtual string ControlType
        {
            get { return _control; }
            set { _control = value; }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            switch (_control)
            {
                case "CheckBox" :
                    CtrlField = new CheckBox() { ID = _controlId + this.ID, Checked = _show};
                    break;
                case "LongText" :
                    CtrlField = new TextBox { ID = _controlId + this.ID , TextMode = TextBoxMode.MultiLine, Rows = 5};
                    if (_required)
                        ((TextBox)CtrlField).ValidationGroup = _validationGroup;
                    break;
                case "Lookup" :
                    CtrlField = new DropDownList() { ID = _controlId + this.ID };
                    if (_required)
                        ((DropDownList)CtrlField).ValidationGroup = _validationGroup;
                    break;
                case "CountryLookup" :
                    CtrlField = new CountryDropDownList() { ID = _controlId + this.ID };
                    if (_required)
                        ((CountryDropDownList)CtrlField).ValidationGroup = _validationGroup;
                    break;
                case "GenderLookup" :
                    CtrlField = new GenderDropDownList() { ID = _controlId + this.ID };
                    if (_required)
                        ((GenderDropDownList)CtrlField).ValidationGroup = _validationGroup;
                    break;
                case "DatePicker" :
                    CtrlField = new DobPicker { ID = _controlId + this.ID };
                    break;
                case "TimeZoneLookup" :
                    CtrlField = new TimeZoneListBox() { ID = _controlId + this.ID };
                    if (_required)
                        ((TimeZoneListBox)CtrlField).ValidationGroup = _validationGroup;
                    break;
                default :
                    CtrlField = new TextBox { ID = _controlId + this.ID };
                    if (_required)
                        ((TextBox)CtrlField).ValidationGroup = _validationGroup;
                    break;
            }
            
            _lblField = new Label { Text = _text, AssociatedControlID = CtrlField.ID };
            this.Controls.Add(_lblField);

            this.Controls.Add(CtrlField);
            
            if (_required)
            {
                _rfvField = new RequiredFieldValidator
                            {
                                ControlToValidate = _control == "DatePicker" ? ((DobPicker)CtrlField).ValidateControl.ID : CtrlField.ID,
                                InitialValue = _control == "DatePicker" ? DateTime.UtcNow.ToString("yyyy") : "",
                                ErrorMessage = this.InvalidMessage,
                                Display = ValidatorDisplay.Dynamic,
                                EnableClientScript = (this.ClientScript.ToLower() != "false"),
                                ValidationGroup = _validationGroup
                            };
                Image img = new Image() { SkinID = "Error" };
                img.ApplyStyleSheetSkin(this.Page);
                _rfvField.Controls.Add(img);
                this.Controls.Add(_rfvField);
            }

        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }

}
