using System;
using System.Collections;
using System.Linq;
using Snitz.Entities;
using Snitz.IDAL;


namespace Snitz.BLL.modconfig
{
    public class ModController
    {
        public ModInfo ModInfo { get; set; }
        private Hashtable _settings;
        private int _modId;

        public ModController(string modname)
        {
            IForumMod dal = Factory<IForumMod>.Create("ForumMod");
            var mod = dal.GetByName(modname);
            if (mod != null)
            {
                ModInfo = mod.First();
                if (ModInfo != null)
                {
                    _modId = ModInfo.Id;
                    _settings = ModInfo.Settings;
                }
            }
            
        }

        public bool InstallMod()
        {
            try
            {
                IForumMod dal = Factory<IForumMod>.Create("ForumMod");
                dal.Add(ModInfo);
            }
            catch (Exception)
            {
                return false;
            }
            return true;

        }

        public Hashtable GetModSettings()
        {
            return _settings;
        }

        public bool Save()
        {
            IForumMod dal = Factory<IForumMod>.Create("ForumMod");
            dal.Update(ModInfo);
            return true;
        }

        public object GetModSetting(string settingname)
        {
            IForumMod dal = Factory<IForumMod>.Create("ForumMod");
            return dal.GetModValue(_modId, settingname);
        }
    }
}
