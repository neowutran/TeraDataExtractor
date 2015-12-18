using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TeraDataExtractor
{
    public class MonsterExtractor
    {

        private readonly Dictionary<string, Zone> _zones = new Dictionary<string, Zone>();

        public MonsterExtractor()
        {
            ExtractZones();
            BasicMonsterExtractor();
            HpAndIsBossExtract();
            WriteXml();
        }

        private void WriteXml()
        {
            using (var outputFile = new StreamWriter("monsters-EN.xml"))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                outputFile.WriteLine("<Zones>");
                foreach (var zone in _zones.Values)
                {
                    outputFile.Write("<Zone ");
                    outputFile.Write("id=\""+zone.Id+"\" ");
                    outputFile.Write("name=\""+zone.Name+"\" ");
                    outputFile.WriteLine(">");
                    foreach (var monster in zone.Monsters)
                    {
                        outputFile.Write("<Monster ");
                        outputFile.Write("name=\""+monster.Value.Name+"\" ");
                        outputFile.Write("id=\""+monster.Value.Id+"\" ");
                        outputFile.Write(monster.Value.IsBoss ? "isBoss=\"True\" " : "isBoss=\"False\" ");
                        outputFile.Write("hp=\""+monster.Value.Hp+"\" ");

                        outputFile.WriteLine("/>");
                    }
                    outputFile.WriteLine("</Zone>");
                }
                outputFile.WriteLine("</Zones>");
                outputFile.Flush();
                outputFile.Close();
            }
        }

        private void ExtractZones()
        {
            var xml = XDocument.Load("E:/TeraDataTools/DataTools/bin/Debug/data/xml/StrSheet_ZoneName.xml");
            foreach (var skill in xml.Root.Elements("String"))
            {
                var id = skill.Attribute("id");
                var name = skill.Attribute("string");
                _zones.Add(id.Value, new Zone(id.Value, name.Value));
                
            }
        }

        private void BasicMonsterExtractor()
        {
            var xml = XDocument.Load("E:/TeraDataTools/DataTools/bin/Debug/data/xml/StrSheet_Creature.xml");

            foreach (var hunting in xml.Root.Elements("HuntingZone"))
            {
                var idZone = hunting.Attribute("id").Value;
             
                foreach (var data in hunting.Elements("String"))
                {
                    var id = data.Attribute("templateId").Value;
                    var name = data.Attribute("name").Value;
                    if (name == "" || id == "" || idZone == "")
                    {
                        continue;
                    }
                    if (!_zones.ContainsKey(idZone))
                    {
                        _zones.Add(idZone, new Zone(idZone, ""));
                    }
                    _zones[idZone].Monsters.Add(id, new Monster(id, name));
                }
             
              
            }
        
        }

        private void HpAndIsBossExtract()
        {
            foreach (
                var file in
                    Directory.EnumerateFiles(@"E:\TeraDataTools\DataTools\bin\Debug\data\xml\NpcData\"))
            {
                var xml = XDocument.Load(file);
                var huntingZone = xml.Root;
              
                    var zoneId = huntingZone.Attribute("huntingZoneId").Value;
                if (!_zones.ContainsKey(zoneId))
                {
                    continue;
                    
                }
                    var zone = _zones[zoneId];
                foreach (var template in huntingZone.Elements("Template"))
                {
                    string isBoss = "False";
                    var dataIsBoss = template.Attribute("showAggroTarget");
                    if (dataIsBoss != null)
                    {
                        isBoss = dataIsBoss.Value;
                        isBoss = isBoss.ToLower();
                    }
                    var hp = template.Element("Stat").Attribute("maxHp").Value;
                    var lvl = template.Element("Stat").Attribute("level").Value;
                    var id = template.Attribute("id").Value;

                    if (!zone.Monsters.ContainsKey(id))
                    {
                        continue;
                    }
                    zone.Monsters[id].Hp = hp;
                    zone.Monsters[id].IsBoss = isBoss == "true";
                }
                
            }
        }

    }
}
