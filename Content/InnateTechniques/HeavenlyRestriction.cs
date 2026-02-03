using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.HeavenlyRestriction;
using sorceryFight.SFPlayer;
using sorceryFight.Content.CursedTechniques.HeavenlyRestriction;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.InnateTechniques
{
    public class HeavenlyRestriction : InnateTechnique
    {
        public override string Name => "HeavenlyRestriction";
        public override string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.Misc.InnateTechniques.{Name}.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new MindlessCarnage(),
            // new InorganicPerception()
        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new Groundshot(),
            new RamCharge(),
            new FlashStep(),
            new LightspeedBarrage()
        };
        public override PlayerDomainExpansion DomainExpansion => null;

        public override void PreUpdate(SorceryFightPlayer sf)
        {
            sf.heavenlyRestriction = true;
            sf.unlockedRCT = false;
        }

        public override void UpdateEquips(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (sf.leftItAllBehind)
            {
                player.GetDamage(DamageClass.Melee) *= 1 + (0.02f * sf.numberBossesDefeated);
                player.GetAttackSpeed(DamageClass.Melee) *= 1 + (0.03f * sf.numberBossesDefeated);
                player.moveSpeed += 0.005f * sf.numberBossesDefeated;
                player.jumpSpeedBoost += 0.01f * sf.numberBossesDefeated;
                player.statDefense += sf.numberBossesDefeated;
            }
            else
            {
                player.GetDamage(DamageClass.Melee) *= 1 + (0.01f * sf.numberBossesDefeated);
                player.GetAttackSpeed(DamageClass.Melee) *= 1 + (0.01f * sf.numberBossesDefeated);
                player.moveSpeed += 0.001f * sf.numberBossesDefeated;
                player.jumpSpeedBoost += 0.005f * sf.numberBossesDefeated;
                player.statDefense += sf.numberBossesDefeated / 2;
            }
        }
    }
}