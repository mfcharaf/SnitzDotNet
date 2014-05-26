using System.Collections.Generic;
using Snitz.Entities;

namespace Snitz.IDAL
{
    public interface ISetup
    {
        string CheckVersion();
        string ExecuteScript(string script);
        bool ChangeDbOwner();
        bool TableExists(string tblName);
        bool FieldExists(string tblName, string fldName);
        bool DatabaseExists();
        IEnumerable<ForumInfo> PrivateForums();
        string[] AllowedMembers(int forumid);
    }
}
