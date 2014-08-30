<%@ WebHandler Language="C#" Class="DownLoadFile" %>
using System;
using System.Web;


public class DownLoadFile : IHttpHandler
{

    #region IHttpHandler Members

    public bool IsReusable
    {
        // Return false in case your Managed Handler cannot be reused for another request.
        // Usually this would be false in case you have some state information preserved per request.
        get { return false; }
    }

    public void ProcessRequest(HttpContext context)
    {
        string file = ""; 
        // get the file name from the querystring 
        if (context.Request.QueryString["FileName"] != null)
        {
            file = context.Request.QueryString["FileName"];
        } 
        string filename = context.Server.MapPath("~/UpLoads/" + file); 
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(filename);
        try
        {
            if (fileInfo.Exists)
            {
                context.Response.Clear();
                context.Response.AddHeader("Content-Disposition", "attachment;filename=\"" + fileInfo.Name + "\"");
                context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                context.Response.ContentType = "application/octet-stream";
                context.Response.TransmitFile(fileInfo.FullName);
                context.Response.Flush();
                LogDownLoad(file);
            }
            else
            {
                throw new Exception("File not found");
            }
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write(ex.Message);
        }
        finally
        {
            context.Response.End();
        } 
    }

    private void LogDownLoad(string file)
    {
        Snitz.BLL.Statistics.FileDownloaded(file);
    }

    #endregion
}

