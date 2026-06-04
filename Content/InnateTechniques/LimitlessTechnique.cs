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
        public override string Name => "Limitless";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Limitless.DisplayName");

        public override Color innateBGColor => new Color(150, 219, 235, 85);

        public override Color innateBorderColor => new Color(0, 0, 0, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new InfinityBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.EyeofCthulhu))
                .SetLockedDescription("Mods.sorceryFight.Buffs.Infinity.LockedDescription"),

            new AmplifiedAuraBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.SkeletronHead))
                .SetLockedDescription("Mods.sorceryFight.Buffs.AmplifiedAuraBuff.LockedDescription"),

            new MaximumAmplifiedAuraBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedMechBossThree)
                .SetLockedDescription("Mods.sorceryFight.Buffs.MaximumAmplifiedAuraBuff.LockedDescription"),
                
            new HollowWickerBasketBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.HallowBoss))
                .SetLockedDescription("Mods.sorceryFight.Buffs.HollowWickerBasketBuff.LockedDescription")
        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new AmplificationBlue()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedEvilBoss)
                .SetLockedDescription("Mods.sorceryFight.CursedTechniques.AmplificationBlue.LockedDescription"),

            new MaximumOutputBlue()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.WallofFlesh))
                .SetLockedDescription("Mods.sorceryFight.CursedTechniques.MaximumOutputBlue.LockedDescription"),

            new ReversalRed()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.Plantera))
                .SetLockedDescription("Mods.sorceryFight.CursedTechniques.ReversalRed.LockedDescription"),

            new MaximumOutputRed()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.Golem))
                .SetLockedDescription("Mods.sorceryFight.CursedTechniques.MaximumOutputRed.LockedDescription"),

            new HollowPurple()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.CultistBoss))
                .SetLockedDescription("Mods.sorceryFight.CursedTechniques.HollowPurple.LockedDescription"),

            new HollowPurple200Percent()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.HasDefeatedBoss(NPCID.MoonLordCore))
                .SetLockedDescription("Mods.sorceryFight.CursedTechniques.HollowPurple200Percent.LockedDescription")
        };

        public override PlayerDomainExpansion DomainExpansion => new UnlimitedVoid();
    }
}