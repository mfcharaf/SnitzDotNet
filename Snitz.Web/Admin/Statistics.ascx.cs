/*'
#################################################################################
## Snitz Forums .net
#################################################################################
## Copyright (C) 2006-07 Huw Reddick, Michael Anderson, Pierre Gorissen and Richard Kinser
## All rights reserved.
## http://forum.snitz.com
##
## Redistribution and use in source and binary forms, with or without
## modification, are permitted provided that the following conditions
## are met:
## 
## - Redistributions of source code and any outputted HTML must retain the above copyright
## notice, this list of conditions and the following disclaimer.
## 
## - The "powered by" text/logo with a link back to http://forum.snitz.com in the footer of the 
## pages MUST remain visible when the pages are viewed on the internet or intranet.
##
## - Neither Snitz nor the names of its contributors/copyright holders may be used to endorse 
## or promote products derived from this software without specific prior written permission. 
## 
##
## THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
## "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
## LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
## FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE 
## COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
## INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES INCLUDING,
## BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; 
## LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER 
## CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
## LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
## ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
## POSSIBILITY OF SUCH DAMAGE.
##
#################################################################################
*/
using System;
using System.IO;
using System.Web.UI;


public partial class Admin_Statistics : UserControl
{

    protected void Page_Load(object sender, EventArgs e)
    {
        lblDBSize.Text = SnitzData.Util.GetDBSize();
        lblAvatar.Text = GetAvatarInfo();
        lblFiles.Text = GetTotalFiles();
        lblForumVersion.Text = SnitzBase.Version.Current;

    }
    /// Returns the size of the directory in bytes
    
    private static long Size(DirectoryInfo dirInfo)
    {
        long total = 0;

        foreach (FileInfo file in dirInfo.GetFiles())
            total += file.Length;

        foreach (DirectoryInfo dir in dirInfo.GetDirectories())
            total += Size(dir);

        return total;
    }

    private string GetAvatarInfo()
    {
        string path = Server.MapPath("~/Avatars");
        DirectoryInfo di = new DirectoryInfo(path);
        return string.Format("{0} KB", (Size(di)/1024));
    }
    private string GetTotalFiles()
    {
        string path = Server.MapPath("~/");
        DirectoryInfo di = new DirectoryInfo(path);
        return ((double)Size(di) / (1024 * 1024)).ToString("#.##") + " MB";
    }

}
