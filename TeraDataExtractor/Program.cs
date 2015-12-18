using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TeraDataExtractor
{
    public class Program
    {

        private Dictionary<int, Zone> _zones = new Dictionary<int, Zone>(); 


        private static void Main(string[] args)
        {
            //MonsterExtractor();
            //MonsterSeparator();
            var monsterExtractor = new MonsterExtractor();

        }

   
        private static void SkillSeparator()
        {
               var reader = new StreamReader(File.OpenRead("skills.csv"));
            StreamWriter mystic = new StreamWriter("mystic.csv");
            StreamWriter priest = new StreamWriter("priest.csv");
            StreamWriter gunner = new StreamWriter("gunner.csv");
            StreamWriter reaper = new StreamWriter("reaper.csv");
            StreamWriter archer = new StreamWriter("archer.csv");
            StreamWriter warrior = new StreamWriter("warrior.csv");
            StreamWriter slayer = new StreamWriter("slayer.csv");
            StreamWriter berserker = new StreamWriter("berserker.csv");
            StreamWriter sorcerer = new StreamWriter("sorcerer.csv");
            StreamWriter lancer = new StreamWriter("lancer.csv");
            StreamWriter common = new StreamWriter("common.csv");
            StreamWriter brawler = new StreamWriter("brawler.csv");

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
                var str = id + "\t" + name + "\t\t";

                if (race != "Common" || gender != "Common")continue;

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

        private static void SkillExtractor()
        {
           

            List<String> alldata = new List<string>();

            foreach (
                var file in
                    Directory.EnumerateFiles(@"E:\TeraDataTools\DataTools\bin\Debug\data\xml\StrSheet_UserSkill\"))
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

                using (StreamWriter outputFile = new StreamWriter("skills.csv"))
                {
                    foreach (string line in alldata)
                    {
                        outputFile.WriteLine(line);
                    }
                }

            }
        }
        
    }
}
