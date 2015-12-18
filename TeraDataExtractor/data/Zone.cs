using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeraDataExtractor
{
    public class Zone
    {
        public Zone(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }

        public Dictionary<string, Monster> Monsters { get; } = new Dictionary<string, Monster>();
    }
}
