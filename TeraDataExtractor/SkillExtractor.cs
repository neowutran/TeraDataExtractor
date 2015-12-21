using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace TeraDataExtractor
{

    /**
    WARNING: Many skill are not reconized (chained skills, etc)
        Pets are not reconized
        Mystics mote are not reconized
        Many thing are not reconized. 
        In the ShinraMeter, most of the data have been manually added
    */
    public class SkillExtractor
    {
        private readonly string _region;

        public SkillExtractor(string region)
        {
            _region = region;
            RawExtract();
            SkillsFormat();
        }


        private void SkillsFormat()
        {
            var reader = new StreamReader(File.OpenRead("skills-" + _region + ".csv"));
            var mystic = new StreamWriter("mystic-" + _region + ".csv");
            var priest = new StreamWriter("priest-" + _region + ".csv");
            var gunner = new StreamWriter("gunner-" + _region + ".csv");
            var reaper = new StreamWriter("reaper-" + _region + ".csv");
            var archer = new StreamWriter("archer-" + _region + ".csv");
            var warrior = new StreamWriter("warrior-" + _region + ".csv");
            var slayer = new StreamWriter("slayer-" + _region + ".csv");
            var berserker = new StreamWriter("berserker-" + _region + ".csv");
            var sorcerer = new StreamWriter("sorcerer-" + _region + ".csv");
            var lancer = new StreamWriter("lancer-" + _region + ".csv");
            var common = new StreamWriter("common-" + _region + ".csv");
            var brawler = new StreamWriter("brawler-" + _region + ".csv");

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (line == null) continue;
                var values = line.Split(';');
                var id = values[0];
                var race = values[1];
                var gender = values[2];
                var playerclass = values[3];
                var name = values[4];
                var str = id + "," + name;

                if (race != "Common" || gender != "Common") continue;

                switch (playerclass)
                {
                    case "Mystic":
                        mystic.WriteLine(str);
                        break;
                    case "Priest":
                        priest.WriteLine(str);
                        break;
                    case "Reaper":
                        reaper.WriteLine(str);
                        break;
                    case "Gunner":
                        gunner.WriteLine(str);
                        break;
                    case "Archer":
                        archer.WriteLine(str);
                        break;
                    case "Warrior":
                        warrior.WriteLine(str);
                        break;
                    case "Slayer":
                        slayer.WriteLine(str);
                        break;
                    case "Berserker":
                        berserker.WriteLine(str);
                        break;
                    case "Sorcerer":
                        sorcerer.WriteLine(str);
                        break;
                    case "Lancer":
                        lancer.WriteLine(str);
                        break;
                    case "Brawler":
                        brawler.WriteLine(str);
                        break;
                    case "Common":
                        common.WriteLine(str);
                        break;
                }
            }
            brawler.Flush();
            brawler.Close();
            common.Flush();
            common.Close();
            mystic.Flush();
            mystic.Close();
            warrior.Flush();
            warrior.Close();
            priest.Flush();
            priest.Close();
            lancer.Flush();
            lancer.Close();
            sorcerer.Flush();
            sorcerer.Close();
            berserker.Flush();
            berserker.Close();
            slayer.Flush();
            slayer.Close();
            archer.Flush();
            archer.Close();
            gunner.Flush();
            gunner.Close();
            reaper.Flush();
            reaper.Close();
        }

        private void RawExtract()
        {
            var alldata = new List<string>();
            var outputFile = new StreamWriter("skills-" + _region + ".csv");
            foreach (
                var file in
                    Directory.EnumerateFiles("E:/TeraData/" + _region + "/StrSheet_UserSkill/"))
            {
                var xml = XDocument.Load(file);
                foreach (var skill in xml.Root.Elements("String"))
                {
                    var id = skill.Attribute("id");
                    var race = skill.Attribute("race");
                    var gender = skill.Attribute("gender");
                    var entityclass = skill.Attribute("class");


                    var name = skill.Attribute("name");
                    if (name == null || entityclass == null || gender == null || race == null || id == null) continue;

                    if (entityclass.Value == "Elementalist")
                    {
                        entityclass.Value = "Mystic";
                    }
                    if (entityclass.Value == "Engineer")
                    {
                        entityclass.Value = "Gunner";
                    }
                    if (entityclass.Value == "Soulless")
                    {
                        entityclass.Value = "Reaper";
                    }
                    if (entityclass.Value == "Fighter")
                    {
                        entityclass.Value = "Brawler";
                    }
                    alldata.Add(id.Value + ";" + race.Value + ";" + gender.Value + ";" + entityclass.Value + ";" +
                                name.Value);
                }


                foreach (var line in alldata)
                {
                    outputFile.WriteLine(line);
                }
            }
            outputFile.Flush();
            outputFile.Close();
        }
    }
}