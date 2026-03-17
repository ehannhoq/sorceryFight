using System.Collections.Generic;
using CalamityMod;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.StarRage;
//using sorceryFight.Content.Buffs.StarRage;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.Buffs.StarRage;

namespace sorceryFight.Content.InnateTechniques
{
    public class StarRageTechnique : InnateTechnique
    {
        public override string Name => "StarRage";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.StarRage.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new SummonGarudaBuff(),
            new FallingBlossomEmotionBuff()
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new GarudaKick()
        };

        public override void PreUpdate(SorceryFightPlayer sf)
        {
            sf.noInnateDomain = true;
        }

        public override PlayerDomainExpansion DomainExpansion => null;


    }
}