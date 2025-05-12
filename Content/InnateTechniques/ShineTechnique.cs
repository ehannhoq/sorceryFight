using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Shrine;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;

namespace sorceryFight.Content.InnateTechniques
{
    public class ShrineTechnique : InnateTechnique
    {
        public override string Name => "Shrine";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Shrine.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new DomainAmplificationBuff(),
            new HollowWickerBasketBuff()
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new Dismantle(),
            new Cleave(),
            new InstantDismantle(),
            new DivineFlame(),
            new WorldCuttingSlash()
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new MalevolentShrine();

        public override void UpdateEquips(SorceryFightPlayer sf)
        {
            sf.Player.moveSpeed += 0.02f * sf.sukunasFingerConsumed;
        }
    }
}
