using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Graphics.Effects;

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

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.CultistBoss);
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<MindlessCarnage>(), 2);

            if (Main.myPlayer != player.whoAmI) return;
            
            if (!Filters.Scene["SF:GaussianBlur"].IsActive())
            {
                Filters.Scene.Activate("SF:GaussianBlur");
            }
            else
            {
                Filters.Scene["SF:GaussianBlur"].GetShader().UseOpacity(1f).UseTargetPosition(player.Center);
            }
        }

        public override void Remove(Player player)
        {
            if (Main.myPlayer != player.whoAmI) return;

            if (Filters.Scene["SF:GaussianBlur"].IsActive())
            {
                Filters.Scene.Activate("SF:GaussianBlur").GetShader().UseOpacity(0f);
                Filters.Scene["SF:GaussianBlur"].Deactivate();
                CameraController.ResetCameraPosition();
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 cameraOffset = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-2, 2));
                CameraController.SetCameraPosition(player.Center + cameraOffset);
            }

            player.AddBuff(BuffID.Dangersense, 2);
            player.AddBuff(BuffID.Hunter, 2);

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

            player.moveSpeed += ((maxSpeed - minSpeed) / 2 * (npcDamage / 600f)) + ((maxSpeed - minSpeed) / 2 * (npcHealth / 100000f)) + minSpeed;
            player.GetDamage(DamageClass.Melee) *= ((maxDamageBoost - minDamageBoost) / 2 * (npcDamage / 600f)) + ((maxDamageBoost - minDamageBoost) / 2 * (npcHealth / 100000f)) + minDamageBoost;
        }
    }
}