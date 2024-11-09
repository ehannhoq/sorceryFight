using System;
using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.PassiveTechniques;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.InnateTechniques
{
    public class InnateTechnique()
    {
        public virtual string Name { get; set; } = "No Innate Technique";
        public virtual List<PassiveTechnique> PassiveTechniques { get; set; } = new List<PassiveTechnique> {};
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

        public static List<InnateTechnique> InnateTechniques
        {
            get
            {
                return new List<InnateTechnique>
                {
                    new LimitlessTechnique(),
                    new ShrineTechnique()
                };
            }
        }

        public virtual void PostUpdate(SorceryFightPlayer sf) {}
    }
}