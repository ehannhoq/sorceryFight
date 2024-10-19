using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.PassiveTechniques;
using sorceryFight.Content.PassiveTechniques.Limitless;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.InnateTechniques
{
    public class LimitlessTechnique : InnateTechnique
    {
        public bool hasInfinity = false;
        public override string Name { get; set; } = "Limitless";
        public override List<PassiveTechnique> PassiveTechniques { get; set; } = new List<PassiveTechnique>
        {
            new Infinity() // 5% Mastery
        };
        public override List<CursedTechnique> CursedTechniques { get; set; } = new List<CursedTechnique>
        {
            new AmplificationBlue(), // 0% Mastery
            new MaximumOutputBlue(), // 10% Mastery

            new ReversalRed(), // 40% Mastery
            new HollowPurple(), // 60% Mastery

            new HollowPurple200Percent() // 80% Mastery

            // new UnlimitedVoid() // 100% Mastery
        };

        public override bool IsValid { get; set; } = true;
    }
}