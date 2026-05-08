using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.IceFormation;
//using sorceryFight.Content.Buffs.StarRage;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques.TenShadows;
using sorceryFight.Utilities;

namespace sorceryFight.Content.InnateTechniques
{
    public class TenShadowsTechnique : InnateTechnique
    {
        public override string Name => "TenShadows";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.TenShadows.DisplayName");

        public override Color innateBGColor => new Color(11, 13, 30, 70);

        public override Color innateBorderColor => new Color(18, 61, 116, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {

        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new RoundDeer(),
            new Nue(),
            new MaxElephant()
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new HeavensRime();


    }
}