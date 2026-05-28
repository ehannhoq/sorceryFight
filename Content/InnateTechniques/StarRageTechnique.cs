using System.Collections.Generic;
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
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using Microsoft.Xna.Framework;
using sorceryFight.Utilities;

namespace sorceryFight.Content.InnateTechniques
{
    public class StarRageTechnique : InnateTechnique
    {
        public override string Name => "StarRage";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.StarRage.DisplayName");

        public override Color innateBGColor => new Color(11, 13, 30, 70);

        public override Color innateBorderColor => new Color(18, 61, 116, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new SummonGarudaBuff(),
            new FallingBlossomEmotionBuff()
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new StarChannel(),
            new GarudaKick(),
            new GarudaWhip(),
            new MassPunch(),
            //new MassKick(),
            //Removed to combine with GarudaKick
            new StarRush()
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new StarCastle();


    }
}