using System.Collections.Generic;
using CalamityMod;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using sorceryFight.Content.Buffs.BloodManipulation;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using sorceryFight.Content.Buffs.Limitless;

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
            new FallingBlossomEmotionBuff()
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new PiercingBlood(),
            new SlicingExorcism(),
            new UnlimitedPiercingBlood(),
            new SuperNova()
        };

        public override void PreUpdate(SorceryFightPlayer sf)
        {
            sf.noInnateDomain = true;
        }

        public override PlayerDomainExpansion DomainExpansion => null;


    }
}