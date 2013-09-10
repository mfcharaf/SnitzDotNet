
<%@ Page Language="C#" AutoEventWireup="true"%>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Forum Error</title>
 <style type="text/css">
     body
    {
        background-color:white;
        font-family: Tahoma, Arial, Helvetica, sans-serif;
        font-size:0.8em;
        color:#191970;
    }
    h3{
	    font-size:Medium;
    }
 </style>
 </head>
 <body>
 <div style="width:80%;margin:auto;text-align:center;">
 <%

 // retrieves query string values
 string name = Page.Request.QueryString["aspxerrorpath"];
 Page.Response.Write("<h3>The was an error on the page: "+name+"</h3>");
 
 
 %>
 <a href="/default.aspx" title="return to home page">Return to forum</a>
 </div>
 </body>
 </html>
 <script type="text/C#">
 public void Page_Error(object sender,EventArgs e)
{
	Exception objErr = Server.GetLastError().GetBaseException();
	string err =	"<b>Error Caught in Page_Error event</b><hr><br>" + 
			"<br><b>Error in: </b>" + Request.Url.ToString() +
			"<br><b>Error Message: </b>" + objErr.Message.ToString()+
			"<br><b>Stack Trace:</b><br>" + 
	                  objErr.StackTrace.ToString();
	Response.Write(err.ToString());
	Server.ClearError();
}

 
 
 </script>