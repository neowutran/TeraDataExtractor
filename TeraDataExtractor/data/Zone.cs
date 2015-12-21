using System.Collections.Generic;

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