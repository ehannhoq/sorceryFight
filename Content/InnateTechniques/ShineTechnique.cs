using System;
using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Content.DomainExpansions;

namespace sorceryFight.Content.InnateTechniques
{
    public class ShrineTechnique : InnateTechnique
    {
        public override string Name => "Shrine";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Shrine.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques => new()
        {
            
        };

        public override List<CursedTechnique> CursedTechniques => new()
        {
            new Dismantle()
        };

        public override DomainExpansion DomainExpansion => new UnlimitedVoid();
    }
}
