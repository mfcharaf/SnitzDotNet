using System.IO;
using System.Web;

namespace SnitzCommon
{

    public class TemplateViewManager
    {
        public static ContentsResponse RenderView(string path)
        {
            return RenderView(path, null);
        }

        public static ContentsResponse RenderView(string path, object data)
        {
            TemplatePage pageHolder = new TemplatePage();
            object test = pageHolder.LoadControl(path);
            TemplateUserControl viewControl = 
                (TemplateUserControl)test;

            if (viewControl == null)
                return ContentsResponse.Empty;

            if (data != null)
            {
                viewControl.Data = data;
            }

            pageHolder.Controls.Add(viewControl);

            string result = "";
            using (StringWriter output = new StringWriter())
            {
                HttpContext.Current.Server.Execute(pageHolder, output, false);
                result = output.ToString();
            }

            return new ContentsResponse(
                    result, 
                    viewControl.StartupScript, 
                    viewControl.CustomStyleSheet
                    );
        }
    }
}