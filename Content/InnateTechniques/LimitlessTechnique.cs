using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.PassiveTechniques;
using sorceryFight.Content.PassiveTechniques.Limitless;

namespace sorceryFight.Content.InnateTechniques
{
    public class LimitlessTechnique : InnateTechnique
    {
        public override string Name { get; set; } = "Limitless";
        public override List<CursedTechnique> CursedTechniques { get; set; } = new List<CursedTechnique>
        {
            new NeutralInfinity(), // !!TEMPORARY!!
            new AmplificationBlue(), // 0% Mastery
            new MaximumOutputBlue(), // 10% Mastery, Post WoF

            new ReversalRed(), // 40% Mastery,
            new HollowPurple(),

            new HollowPurple200Percent()

            // new UnlimitedVoid() // 100% Mastery
        };

        public override bool IsValid { get; set; } = true;
    }
}