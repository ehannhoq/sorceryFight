using System.Collections.Generic;
using rail;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.PassiveTechniques;

namespace sorceryFight.Content.InnateTechniques
{
    public class InnateTechnique()
    {
        public virtual string Name { get; set; } = "No Innate Technique";
        public virtual List<CursedTechnique> CursedTechniques { get; set; } = new List<CursedTechnique> {};

        public virtual bool IsValid { get; set; } = false;

        public static InnateTechnique GetInnateTechnique(string name)
        {
            switch (name)
            {
                case "Limitless":
                    return new LimitlessTechnique();
            }

            return new InnateTechnique();
        }
    }
}