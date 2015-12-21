using System.Collections.Generic;

namespace TeraDataExtractor
{
    public class Program
    {
        private static void Main(string[] args)
        {
            new MonsterExtractor("NA");
            new MonsterExtractor("EU-EN");
            new MonsterExtractor("EU-FR");
            new MonsterExtractor("EU-GER");

         //   new SkillExtractor("NA");
         //   new SkillExtractor("EU-EN");
         //   new SkillExtractor("EU-FR");
         //   new SkillExtractor("EU-GER");
        }
    }
}