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

        public override Color innateBGColorOverride => new Color(20, 5, 5, 220);

        public override Color innateBorderColorOverride => new Color(180, 20, 20, 200);
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