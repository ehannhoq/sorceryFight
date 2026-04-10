using System.Collections.Generic;
using CalamityMod;
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
            new FlowingRedScaleBuff(),
            new FlowingRedScaleStackBuff()
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new SoulDismantle(),
            new PiercingBlood(),
            new ChainDismantle(),
            new BloodDaggerStorm(),
            new LineDevestation(),
            new FullIncarnationKOC()
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new Home();

        public override void UpdateEquips(SorceryFightPlayer sf)
        {
            sf.Player.GetDamage(DamageClass.Melee) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Ranged) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Magic) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Summon) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(RogueDamageClass.Throwing) *= 1 + (0.05f * sf.sukunasFingerConsumed);

            sf.Player.statDefense *= 1 + (0.03f * sf.sukunasFingerConsumed);
    
            sf.blackFlashWindowTime += 1;
        }

        public override void UpdateLifeRegen(SorceryFightPlayer sf) 
        {
            sf.Player.lifeRegen += 2 * sf.sukunasFingerConsumed;
        }
    }
}