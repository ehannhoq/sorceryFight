using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Shrine;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;

namespace sorceryFight.Content.InnateTechniques
{
    public class ShrineTechnique : InnateTechnique
    {
        public override string Name => "Shrine";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Shrine.DisplayName");
        //public override Color innateBGColor => new Color(98, 4, 4, 70);
        public override Color innateBGColor => new Color(169, 4, 4, 85);

        public override Color innateBorderColor => new Color(0, 0, 0, 128);
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

    }
}
