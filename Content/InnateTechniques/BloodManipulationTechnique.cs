using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using sorceryFight.Content.Buffs.BloodManipulation;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using sorceryFight.Content.Buffs.Limitless;
using Microsoft.Xna.Framework;
using sorceryFight.Utilities;
using Terraria.ID;

namespace sorceryFight.Content.InnateTechniques
{
    public class BloodManipulationTechnique : InnateTechnique
    {
        public override string InternalName => "BloodManipulation";

        //public override Color innateBGColor => new Color(205, 205, 205, 70);
        public override Color innateBGColor => new Color(156, 14, 134, 110);
        public override Color innateBorderColor => new Color(120, 40, 160, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new FlowingRedScale()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedEvilBoss)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.EvilBoss"),

            new FlowingRedScaleStack()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedMechBossThree)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.MechBossThree"),

            new FallingBlossomEmotion()
                .SetUnlock(NPCID.HallowBoss)
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new SlicingExorcism()
                .SetUnlock(NPCID.EyeofCthulhu),

            new SelfBloodBlade()
                .SetUnlock(NPCID.SkeletronHead),

            new PiercingBlood()
                .SetUnlock(NPCID.WallofFlesh),

            new BloodDaggerStorm()
             .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.unlockedRCT)
             .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.RCT"),

            new SuperNova()
                .SetUnlock(NPCID.Golem),

            new UnlimitedPiercingBlood()
                .SetUnlock(NPCID.CultistBoss)
        };

        public override void PreUpdate(SorceryFightPlayer sf)
        {
            base.PreUpdate(sf);
            sf.noInnateDomain = true;
        }

        public override PlayerDomainExpansion DomainExpansion => null;


    }
}
