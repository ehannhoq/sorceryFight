using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using sorceryFight.Content.CursedTechniques.Vessel;
using sorceryFight.Content.Buffs.BloodManipulation;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using sorceryFight.Utilities;

namespace sorceryFight.Content.InnateTechniques
{
    public class VesselTechnique : InnateTechnique
    {
        public override string InternalName => "Vessel";

        public override Color innateBGColor => new Color(236, 171, 162, 100);

        public override Color innateBorderColor => new Color(64, 76, 140, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new FlowingRedScale()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 5)
                .SetUnlockRequirement(SFUtils.GetLocalization("Mods.sorceryFight.UnlockRequirements.SukunasFingers").WithFormatArgs(5).Value),

            new FlowingRedScaleStack()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 15)
                .SetUnlockRequirement(SFUtils.GetLocalization("Mods.sorceryFight.UnlockRequirements.SukunasFingers").WithFormatArgs(15).Value),
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new SoulDismantle()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 1)
                .SetUnlockRequirement(SFUtils.GetLocalization("Mods.sorceryFight.UnlockRequirements.SukunasFingers").WithFormatArgs(1).Value),

            new PiercingBlood()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 3)
                .SetUnlockRequirement(SFUtils.GetLocalization("Mods.sorceryFight.UnlockRequirements.SukunasFingers").WithFormatArgs(3).Value),

            new ChainDismantle()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 10)
                .SetUnlockRequirement(SFUtils.GetLocalization("Mods.sorceryFight.UnlockRequirements.SukunasFingers").WithFormatArgs(10).Value),

            new BloodDaggerStorm()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 12)
                .SetUnlockRequirement(SFUtils.GetLocalization("Mods.sorceryFight.UnlockRequirements.SukunasFingers").WithFormatArgs(12).Value),

            new LineDevestation()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 20)
                .SetUnlockRequirement(SFUtils.GetLocalization("Mods.sorceryFight.UnlockRequirements.SukunasFingers").WithFormatArgs(20).Value),

            new FullIncarnationKOC()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasSkull)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.SukunasSkull")
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new Home();

        public override void UpdateEquips(SorceryFightPlayer sf)
        {
            sf.Player.GetDamage(DamageClass.Melee) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Ranged) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Magic) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Summon) *= 1 + (0.05f * sf.sukunasFingerConsumed);

            sf.Player.statDefense *= 1 + (0.03f * sf.sukunasFingerConsumed);

            sf.blackFlashWindowTime += 1;
        }

        public override void UpdateLifeRegen(SorceryFightPlayer sf)
        {
            sf.Player.lifeRegen += 2 * sf.sukunasFingerConsumed;
        }
    }
}
