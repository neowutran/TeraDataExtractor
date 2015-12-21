using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TeraDataExtractor
{
    /*
     If you are here, you will need to read and understand the game database.
     Welcome in hell.
    */

    public class MonsterExtractor
    {
        private readonly List<string> _battlefields = new List<string>();
        private readonly string _region;

        private readonly Dictionary<string, Zone> _zones = new Dictionary<string, Zone>();
        private const string RootFolder = "E:/TeraData/";

        public MonsterExtractor(string region)
        {
            _region = region;
            ExtractZones();
            BasicMonsterExtractor();
            HpAndIsBossExtract();
            WriteXml();
        }

        private void WriteXml()
        {
            using (var outputFile = new StreamWriter("monsters-" + _region + ".xml"))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                outputFile.WriteLine("<Zones>");
                foreach (var zone in _zones.Values)
                {
                    if (zone.Monsters.Count == 0)
                    {
                        continue;
                    }
                    outputFile.Write("<Zone ");
                    outputFile.Write("id=\"" + zone.Id + "\" ");
                    outputFile.Write("name=\"" + zone.Name + "\" ");
                    outputFile.WriteLine(">");
                    foreach (var monster in zone.Monsters)
                    {
                        outputFile.Write("<Monster ");
                        outputFile.Write("name=\"" + monster.Value.Name + "\" ");
                        outputFile.Write("id=\"" + monster.Value.Id + "\" ");
                        outputFile.Write(monster.Value.IsBoss ? "isBoss=\"True\" " : "isBoss=\"False\" ");
                        outputFile.Write("hp=\"" + monster.Value.Hp + "\" ");

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
            DungeonZone();
            ChannelingAndOpenworldZone();
            BattlefieldZone();

            var xml = XDocument.Load(RootFolder + _region + "/StrSheet_ZoneName.xml");
            foreach (var zone in xml.Root.Elements("String"))
            {
                var id = zone.Attribute("id");
                var name = zone.Attribute("string");
                if (_zones.ContainsKey(id.Value))
                {
                    continue;
                }
                _zones.Add(id.Value, new Zone(id.Value, name.Value));
            }
        }


        private void BattlefieldZone()
        {
         /**
            Extract battlefield.
            No Boss in battlefield.
        */

            var continentData = XDocument.Load(RootFolder + _region + "/ContinentData.xml");
            var continents = new Dictionary<string, List<string>>();
            foreach (var continent in continentData.Root.Elements("Continent"))
            {
                if (continent.Attribute("channelType").Value != "battleField")
                {
                    continue;
                }
                var id = continent.Attribute("id").Value;

                var huntingZone = continent.Element("HuntingZone");

                var huntingZoneId = huntingZone.Attribute("id").Value;
                if (!continents.ContainsKey(id))
                {
                    continents.Add(id, new List<string> {huntingZoneId});
                    continue;
                }
                continents[id].Add(huntingZoneId);
            }

            var zoneId = new Dictionary<string, string>();
            foreach (
                var file in
                    Directory.EnumerateFiles(RootFolder + _region + "/Area/"))
            {
                var xml = XDocument.Load(file);
                var continentId = xml.Root.Attribute("continentId").Value;
                var id = xml.Root.Attribute("nameId").Value;
                if (zoneId.ContainsKey(id))
                {
                    continue;
                }
                zoneId.Add(id, continentId);
            }

            var xml2 = XDocument.Load("E:/TeraData/" + _region + "/StrSheet_Region.xml");
            foreach (var region in xml2.Root.Elements("String"))
            {
                var id = region.Attribute("id").Value;
                var name = region.Attribute("string").Value;
                if (!zoneId.ContainsKey(id))
                {
                    continue;
                }
                var continentId = zoneId[id];
                if (!continents.ContainsKey(continentId)) continue;
                foreach (var huntingZone in continents[continentId])
                {
                    if (_zones.ContainsKey(huntingZone)) continue;
                    _zones.Add(huntingZone, new Zone(huntingZone, name));
                    _battlefields.Add(huntingZone);
                }
            }
        }

        private void ChannelingAndOpenworldZone()
        {
            var continentData = XDocument.Load(RootFolder + _region + "/ContinentData.xml");
            var continents = new Dictionary<string, List<string>>();
            foreach (var continent in continentData.Root.Elements("Continent"))
            {
                if (continent.Attribute("channelType").Value != "channelingZone" &&
                    continent.Attribute("channelType").Value != "none")
                {
                    continue;
                }
                var id = continent.Attribute("id").Value;

                foreach (var huntingZone in continent.Elements("HuntingZone"))
                {
                    var huntingZoneId = huntingZone.Attribute("id").Value;
                    if (!continents.ContainsKey(id))
                    {
                        continents.Add(id, new List<string> {huntingZoneId});
                        continue;
                    }
                    continents[id].Add(huntingZoneId);
                }
            }


            var zoneId = new Dictionary<string, List<string>>();
            foreach (
                var file in
                    Directory.EnumerateFiles(RootFolder + _region + "/Area/"))
            {
                var xml = XDocument.Load(file);
                var continentId = xml.Root.Attribute("continentId").Value;
                var id = xml.Root.Attribute("nameId").Value;
                if (!zoneId.ContainsKey(id))
                {
                   zoneId[id] = new List<string>();
                }
                zoneId[id].Add(continentId);
            }
            var xml2 = XDocument.Load(RootFolder + _region + "/StrSheet_Region.xml");
            foreach (var region in xml2.Root.Elements("String"))
            {
                var id = region.Attribute("id").Value;
                var name = region.Attribute("string").Value;
                if (!zoneId.ContainsKey(id))
                {
                    continue;
                }
                var continentIdList = zoneId[id];
                foreach (var continentId in continentIdList)
                {
                    if (!continents.ContainsKey(continentId)) continue;
                    foreach (var huntingZone in continents[continentId])
                    {
                        if (_zones.ContainsKey(huntingZone)) continue;
                        _zones.Add(huntingZone, new Zone(huntingZone, name));
                    }
                }
              
            }
        }


        private void DungeonZone()
        {
            var xml = XDocument.Load(RootFolder + _region + "/ContinentData.xml");
            var continents = new Dictionary<string, string>();
            foreach (var continent in xml.Root.Elements("Continent"))
            {
                if (continent.Attribute("channelType").Value != "dungeon")
                {
                    continue;
                }
                var id = continent.Attribute("id").Value;
                var huntingZone = continent.Element("HuntingZone");
                var huntingZoneId = huntingZone.Attribute("id").Value;
                continents.Add(id, huntingZoneId);
            }
            xml = XDocument.Load(RootFolder + _region + "/StrSheet_Dungeon/StrSheet_Dungeon-0.xml");
            foreach (var dungeon in xml.Root.Elements("String"))
            {
                var id = dungeon.Attribute("id").Value;
                var name = dungeon.Attribute("string").Value;
                if (!continents.ContainsKey(id))
                {
                    continue;
                }
                var zoneId = continents[id];
                if (!_zones.ContainsKey(zoneId))
                {
                    _zones.Add(zoneId, new Zone(zoneId, name));
                }
            }
        }

        private void BasicMonsterExtractor()
        {
            var xml = XDocument.Load(RootFolder + _region + "/StrSheet_Creature.xml");

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
                    Directory.EnumerateFiles(RootFolder + _region + "/NpcData/"))
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
                    var isBoss = "False";
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
                    if (_battlefields.Contains(zoneId)) continue;
                    zone.Monsters[id].IsBoss = isBoss == "true";
                }
            }
        }
    }
}