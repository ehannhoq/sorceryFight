using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.NPCs.Leviathan;

namespace sorceryFight.Content.Buffs.BloodManipulation
{
    public class FlowingRedScaleStackBuff : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.FlowingRedScaleStackBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.FlowingRedScaleStackBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.FlowingRedScaleStackBuff.LockedDescription");

        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 40f;

        public override float BloodCostPerSecond { get; set; } = -30;

        float BossMultiplier = 1.5f;
        public virtual int DefenseAddition { get; set; } = 36;
        public virtual float DamageNegation { get; set; } = 0.30f;

        public override bool isAura => true;

        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + $"+{DefenseAddition}% defense boost.\n"
                        + $"+{DamageNegation}% damage negation.\n"
                        + $"Generates 30 BE/s \n"
                        + "If you don't have RCT it will consume health instead\n"
                        + "You cannot use Cursed Techniques while this is active,\n"
                        + "unless you have a unique body structure.\n"
                        + $"Flowing Red Scale takes +{BossMultiplier}x more CE during boss fights.\n";
            }
        }


        public Dictionary<int, int> auraIndices;

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<FlowingRedScaleStackBuff>(), 2);
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            foreach (var technique in player.SorceryFight().innateTechnique.PassiveTechniques)
            {
                if (technique.isAura && technique != this)
                    technique.isActive = false;
            }

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            //if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            //{
            //    Vector2 playerPos = player.MountedCenter;
            //    var entitySource = player.GetSource_FromThis();

            //    auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<DomainAmplificationProjectile>(), 0, 0, player.whoAmI);
            //}

            sfPlayer.disableCurseTechniques = true;

        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.domainAmp = false;

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (auraIndices.ContainsKey(player.whoAmI))
            {
                Main.projectile[auraIndices[player.whoAmI]].Kill();
                auraIndices.Remove(player.whoAmI);
            }

            sfPlayer.disableCurseTechniques = false;
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            if (sf.innateTechnique.Name == "Vessel")
            {
                return sf.sukunasFingerConsumed >= 5;
            }
            else
            {
                return sf.HasDefeatedBoss(ModContent.NPCType<Anahita>());
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            player.endurance += DamageNegation;
            player.statDefense += DefenseAddition;
            
            float multiplier = 1;
            if (CalamityMod.CalPlayer.CalamityPlayer.areThereAnyDamnBosses)
            {
                multiplier = BossMultiplier;
            }

            if (sfPlayer.unlockedRCT)
            {
                CostPerSecond = 40f;
                CostPerSecond += CostPerSecond * multiplier;
            }
            else
            {
                CostPerSecond = 0;
                player.lifeRegen -= 40;
            }



            
            base.Update(player, ref buffIndex);
        }
    }
}
