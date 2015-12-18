using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeraDataExtractor
{
    public class Monster
    {
        public Monster(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }

        public string Name { get; }

        public string Hp { get; set; }
        public bool IsBoss { get; set; }
    }
}
