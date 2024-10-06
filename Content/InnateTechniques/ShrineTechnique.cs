using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;


namespace sorceryFight.Content.InnateTechniques
{
    public class ShrineTechnique : InnateTechnique
    {
        public override string Name { get; set; } = "Shrine";
        public override List<CursedTechnique> CursedTechniques { get; set; } = new List<CursedTechnique>
        {
            // new Dismantle()
            // new WorldCuttingSlash() , "Slash That Cuts The World"

            // new Cleave()

            // new DivineFlame()
            
            // new MalevolentShrine()
        };

        public override bool IsValid { get; set; } = true;
    }
}