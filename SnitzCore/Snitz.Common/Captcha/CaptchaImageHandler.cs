using System;
using System.Drawing.Imaging;
using System.Web;
using System.Web.SessionState;

public class CaptchaImageHandler : IHttpHandler, IRequiresSessionState
{
    private Random random = new Random();
    public void ProcessRequest(HttpContext context)
    {
        HttpApplication App = context.ApplicationInstance;
        HttpContext.Current.Session["CaptchaImageText"] = GenerateRandomCode();
        CaptchaImage ci = new CaptchaImage(HttpContext.Current.Session["CaptchaImageText"].ToString(), 200, 50, "Century Schoolbook");

        // Change the response headers to output a JPEG image.
        App.Context.Response.Clear();
        App.Context.Response.ContentType = "image/jpeg";

        // Write the image to the response stream in JPEG format.
        ci.Image.Save(App.Context.Response.OutputStream, ImageFormat.Jpeg);

        // Dispose of the CAPTCHA image object.
        ci.Dispose();
        App.Response.StatusCode = 200;
        context.ApplicationInstance.CompleteRequest();
    }
    private string GenerateRandomCode()
    {
        string x = "ACDEFGHJKLMNPRSTUVWXYZ2345679";
        string s = "";
        int ii = 0;
        for (int i = 0; i < 6; i++)
        {
            ii = this.random.Next(29);
            s = String.Concat(s, x[ii]);
        }
        return s;
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}
