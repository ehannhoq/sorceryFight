using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using sorceryFight.Content.Buffs.BloodManipulation;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using sorceryFight.Content.Buffs.Limitless;
using Microsoft.Xna.Framework;

namespace sorceryFight.Content.InnateTechniques
{
    public class BloodManipulationTechnique : InnateTechnique
    {
        public override string Name => "BloodManipulation";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.BloodManipulation.DisplayName");

        //public override Color innateBGColor => new Color(205, 205, 205, 70);
        public override Color innateBGColor => new Color(156, 14, 134, 110);
        public override Color innateBorderColor => new Color(120, 40, 160, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new FlowingRedScaleBuff(),
            new FlowingRedScaleStackBuff(),
            new FallingBlossomEmotionBuff()
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new SlicingExorcism(),
            new SelfBloodBlade(),
            new PiercingBlood(),
            new BloodDaggerStorm(),
            new SuperNova(),
            new UnlimitedPiercingBlood()
        };

        public override void PreUpdate(SorceryFightPlayer sf)
        {
            sf.noInnateDomain = true;
        }

        public override PlayerDomainExpansion DomainExpansion => null;


    }
}