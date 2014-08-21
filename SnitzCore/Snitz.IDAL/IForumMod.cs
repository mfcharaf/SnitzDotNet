using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Snitz.Entities;

namespace Snitz.IDAL
{
    
    public interface IForumMod : IBaseObject<ModInfo>
    {
        /// <summary>
        /// look up a single mod setting in the database
        /// </summary>
        /// <param name="modid">Id of the mod whose setting you want to retrieve</param>
        /// <param name="setting">Name of the setting</param>
        /// <returns>The value for the setting</returns>
        object GetModValue(int modid, string setting);

    }
}
