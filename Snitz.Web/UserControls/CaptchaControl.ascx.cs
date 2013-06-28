using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SnitzConfig;

public partial class SnitzCaptchaControl : UserControl
{
    public Boolean IsValid{
        get { Page.Validate();
        return CaptchaValidator.IsValid;
        }
    }
    private Boolean _submit;
    public Boolean ShowSubmit{
        get {return _submit;}
        set {_submit = value;}
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        SubmitButton.Visible = _submit;
        Visible = Config.UseCaptcha;
    }
    protected void CaptchaValidator_ServerValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = CodeNumberTextBox.Text == HttpContext.Current.Session["CaptchaImageText"].ToString();
    }
}
