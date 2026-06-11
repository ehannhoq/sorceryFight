using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria.ID;
using sorceryFight.Content.Buffs.Shrine;

namespace sorceryFight.Content.InnateTechniques
{
    public class LimitlessTechnique : InnateTechnique
    {
        public override string InternalName => "Limitless";

        public override Color innateBGColor => new Color(150, 219, 235, 85);

        public override Color innateBorderColor => new Color(0, 0, 0, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new InfinityBuff()
                .SetUnlock(NPCID.EyeofCthulhu),

            new AmplifiedAuraBuff()
                .SetUnlock(NPCID.SkeletronHead),

            new MaximumAmplifiedAuraBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedMechBossThree)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.MechBossThree"),
                
            new HollowWickerBasketBuff()
                .SetUnlock(NPCID.HallowBoss)
        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new AmplificationBlue()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedEvilBoss)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.EvilBoss"),

            new MaximumOutputBlue()
                .SetUnlock(NPCID.WallofFlesh),

            new ReversalRed()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.unlockedRCT)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.RCT"),

            new MaximumOutputRed()
                .SetUnlock(NPCID.Golem),

            new HollowPurple()
                .SetUnlock(NPCID.CultistBoss),

            new HollowPurple200Percent()
                .SetUnlock(NPCID.MoonLordCore)
        };

        public override PlayerDomainExpansion DomainExpansion => new UnlimitedVoid();
    }
}