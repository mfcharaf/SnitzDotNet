using System;
using Snitz.Entities;

namespace Snitz.BLL.modconfig
{
    public abstract class ModConfigBase
    {
        private readonly string _name;
        private ModInfo _modinfo;

        public int Id { get; private set; }
        public string Name { get { return _name; } }
        public string Description { get; private set; }
        public Version Version { get; private set; }

        public ModInfo ModConfiguration { get { return _modinfo; } }
        public bool Enabled { get; set; }

        protected ModConfigBase(string name)
        {
            this._name = name;
            var controller = new ModController(name);
            this._modinfo = controller.ModInfo ?? LoadDefaultConfig(controller);
            if (_modinfo != null)
            {
                Id = _modinfo.Id;
                Description = _modinfo.Description;
                Version = _modinfo.Version;
                Enabled = _modinfo.Enabled;
            }
            else
            {
                throw new Exception(String.Format("{0} configuration data not defined", name));
            }
        }

        /// <summary>
        /// Loads default MOD config parameters if none exist in the database
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        protected abstract ModInfo LoadDefaultConfig(ModController controller);

    }
}
