using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using sorceryFight.Content.CursedTechniques.HeavenlyRestriction;

namespace sorceryFight.Content.InnateTechniques
{
    public class HeavenlyRestriction : InnateTechnique
    {
        public override string Name => "HeavenlyRestriction";
        public override string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.Misc.InnateTechniques.{Name}.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new PebbleBarrage()
        };
        public override PlayerDomainExpansion DomainExpansion => null;

        public override void PreUpdate(SorceryFightPlayer sf)
        {
            sf.heavenlyRestriction = true;
            sf.unlockedRCT = false;
        }
    }
}