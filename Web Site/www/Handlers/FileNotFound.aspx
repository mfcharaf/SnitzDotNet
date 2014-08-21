
<%@ Page Language="C#"%>
 <!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
 <head>
 <title>File not found</title>
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
 Page.Response.Write("<h3>The page you requested: "+name+" could not be found</h3>");
 %>
 <a href="/default.aspx" title="return to home page">Return to forum</a>
 </div>
 </body>
 </html>

