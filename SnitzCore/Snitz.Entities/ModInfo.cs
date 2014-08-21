using System;
using System.Collections;


namespace Snitz.Entities
{
    public class ModInfo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Version Version { get; set; }
        public bool Enabled { get; set; }
        public string Roles { get; set; }
        public string AdminControl { get; set; }
        public Hashtable Settings { get; set; }

    }
}
