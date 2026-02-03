using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Graphics.Effects;
using System;

namespace sorceryFight.Content.Buffs.HeavenlyRestriction
{
    public class MindlessCarnage : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.MindlessCarnage.DisplayName");
        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + "You must be standing still in order to recieve the buffs.\n"
                        + "Grants immunity to enemy domains.\n"
                        + "Grants +20 defense.\n"
                        + "Grants immunity to knockback.\n"
                        + "You cannot use Cursed Techniques while this is active,\n"
                        + "unless you have a unique body structure.\n";
            }
        }
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.MindlessCarnage.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.MindlessCarnage.LockedDescription");
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 85;

        private const float minSpeed = 0.05f;
        private const float maxSpeed = 0.75f;
        private const float minDamageBoost = 1.1f;
        private const float maxDamageBoost = 2f;

        private static float ease = 0.0f;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.WallofFlesh);
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<MindlessCarnage>(), 2);

            if (player.HasBuff<InorganicPerception>())
            {
                player.SorceryFight().innateTechnique.PassiveTechniques[1].isActive = false;
            }

            if (Main.myPlayer != player.whoAmI) return;

            if (!Filters.Scene["SF:MindlessBarrage"].IsActive())
            {
                Filters.Scene.Activate("SF:MindlessBarrage");
            }

            ease = MathHelper.Clamp(ease + 0.04f, 0f, 1f);
            Filters.Scene["SF:MindlessBarrage"].GetShader().UseOpacity(ease).UseTargetPosition(player.Center);
        }

        public override void Remove(Player player)
        {
            if (Main.myPlayer != player.whoAmI) return;

            ease = MathHelper.Clamp(ease - 0.04f, 0f, 1f);

            if (ease > 0)
            {
                Filters.Scene["SF:MindlessBarrage"].GetShader().UseOpacity(ease).UseTargetPosition(player.Center);
                CameraController.ResetCameraPosition();
            }
            else
            {
                Filters.Scene["SF:MindlessBarrage"].Deactivate();
                ease = 0;
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CostPerSecond = 65f;

            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 cameraOffset = new Vector2(Main.rand.NextFloat(-5 * ease, 5 * ease), Main.rand.NextFloat(-2 * ease, 2 * ease));
                CameraController.SetCameraPosition(player.Center + cameraOffset);
            }

            player.AddBuff(BuffID.Dangersense, 2);
            player.AddBuff(BuffID.Hunter, 2);
            player.statDefense /= 0.8f;

            float minDistance = 2000f;
            NPC strongestNPC = null;

            float npcHealth = 0;
            float npcDamage = 0;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.type == NPCID.TargetDummy || !npc.active) continue;

                float dist = (npc.Center - player.Center).Length();
                if (dist < minDistance)
                {
                    if (npcDamage < npc.damage || npcHealth < npc.lifeMax)
                    {
                        strongestNPC = npc;
                        npcHealth = npc.lifeMax;
                        npcDamage = npc.damage;
                    }
                }
            }

            if (strongestNPC == null) return;

            float damageProportion = npcDamage / 600f;
            float healthProportion = npcHealth / 100000f;

            // TODO: if theres a system that identifies the current strongest boss, use that bosses health and contact damage instead of these arbituary numbers.
            player.moveSpeed += ((maxSpeed - minSpeed) / 2 * damageProportion) + ((maxSpeed - minSpeed) / 2 * healthProportion) + minSpeed;
            player.GetDamage(DamageClass.Melee) *= ((maxDamageBoost - minDamageBoost) / 2 * damageProportion) + ((maxDamageBoost - minDamageBoost) / 2 * healthProportion) + minDamageBoost;

            CostPerSecond += 50 * ((damageProportion + healthProportion) / 2);
            base.Update(player, ref buffIndex);
        }
    }
}