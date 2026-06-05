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
        public override string Name => "Vessel";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Vessel.DisplayName");

        public override Color innateBGColor => new Color(236, 171, 162, 100);

        public override Color innateBorderColor => new Color(64, 76, 140, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new FlowingRedScaleBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 5),

            new FlowingRedScaleStackBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 15),
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new SoulDismantle()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 1),

            new PiercingBlood()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 3),

            new ChainDismantle()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 10),

            new BloodDaggerStorm()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 12),

            new LineDevestation()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.sukunasFingerConsumed >= 20),

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