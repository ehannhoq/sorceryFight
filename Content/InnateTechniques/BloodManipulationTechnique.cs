using System.Collections.Generic;
using CalamityMod;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using sorceryFight.Content.CursedTechniques.Vessel;
using sorceryFight.Content.Buffs.BloodManipulation;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;

namespace sorceryFight.Content.InnateTechniques
{
    public class BloodManipulationTechnique : InnateTechnique
    {
        public override string Name => "BloodManipulation";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.BloodManipulation.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new FlowingRedScaleBuff(),
            new FlowingRedScaleStackBuff(),
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new PiercingBlood()
        };

        public override PlayerDomainExpansion DomainExpansion => null;


    }
}