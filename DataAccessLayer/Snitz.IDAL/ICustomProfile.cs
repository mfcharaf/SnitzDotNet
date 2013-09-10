using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snitz.Entities;

namespace Snitz.IDAL
{
    public interface ICustomProfile
    {
        bool AddColumn(ProfileColumn column);
        bool DropColumn(string column);
        List<ProfileColumn> GetColumns();
    }
}
