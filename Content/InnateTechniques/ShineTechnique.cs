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
using Terraria.ID;

namespace sorceryFight.Content.InnateTechniques
{
    public class ShrineTechnique : InnateTechnique
    {
        public override string InternalName => "Shrine";
        //public override Color innateBGColor => new Color(98, 4, 4, 70);
        public override Color innateBGColor => new Color(169, 4, 4, 85);

        public override Color innateBorderColor => new Color(0, 0, 0, 128);
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new DomainAmplificationBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedMechBossThree)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.MechBossThree"),
            new HollowWickerBasketBuff()
                .SetUnlock(NPCID.HallowBoss)
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new Dismantle()
                .SetUnlock(NPCID.EyeofCthulhu),
            new Cleave()
                .SetUnlock(NPCID.SkeletronHead),
            new InstantDismantle()
                .SetUnlock(NPCID.WallofFlesh),
            new DivineFlame()
                .SetUnlock(NPCID.Golem),
            new WorldCuttingSlash()
                .SetUnlock(NPCID.MoonLordCore)
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new MalevolentShrine();

    }
}
