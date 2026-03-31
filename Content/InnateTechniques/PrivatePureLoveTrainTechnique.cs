using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using Microsoft.Xna.Framework;

namespace sorceryFight.Content.InnateTechniques
{
    public class PrivatePureLoveTrainTechnique : InnateTechnique
    {
        public override string Name => "PrivatePureLoveTrain";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.PrivatePureLoveTrain.DisplayName");

        public override Color innateBGColor => new Color(149, 81, 157, 70);

        public override Color innateBorderColor => new Color(0, 0, 0, 128);
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {

        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new PachinkoBalls(),
            new HakarisDoor(),
            new PassingThrough(),
            new CargoCrate(),
            new RailroadSign()
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new IdleDeathGamble();
    }
}