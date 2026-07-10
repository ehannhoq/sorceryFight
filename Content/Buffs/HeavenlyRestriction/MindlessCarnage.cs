using Microsoft.Xna.Framework;
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
        public override string InternalName => "MindlessCarnage";

        public MindlessCarnage()
        {
            Technique.cost = 85;
        }


        private const float MIN_SPEED = 0.05f;
        private const float MAX_SPEED = 0.75f;
        private const float MIN_DAMAGE_BOOST = 1.1f;
        private const float MAX_DAMAGE_BOOST = 2f;

        private static float ease = 0.0f;

        public override void OnApply(Player player)
        {
            player.AddBuff(ModContent.BuffType<MindlessCarnage>(), 2);

            if (Main.myPlayer != player.whoAmI) return;

            if (!Filters.Scene["SF:MindlessBarrage"].IsActive())
            {
                Filters.Scene.Activate("SF:MindlessBarrage");
            }

            // ease = MathHelper.Clamp(ease + 0.04f, 0f, 1f);
            Filters.Scene["SF:MindlessBarrage"].GetShader().UseOpacity(1.0f).UseTargetPosition(player.Center);
        }

        public override void OnRemove(Player player)
        {
            if (Main.myPlayer != player.whoAmI) return;

            // ease = MathHelper.Clamp(ease - 0.04f, 0f, 1f);

            // if (ease > 0)
            // {
            //     Filters.Scene["SF:MindlessBarrage"].GetShader().UseOpacity(ease).UseTargetPosition(player.Center);
            // }
            // else
            // {
            CameraController.ResetCameraPosition();
            Filters.Scene["SF:MindlessBarrage"].Deactivate();
            // ease = 0;
            // }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            Technique.cost = 65f;

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
            player.moveSpeed += ((MAX_SPEED - MIN_SPEED) / 2 * damageProportion) + ((MAX_SPEED - MIN_SPEED) / 2 * healthProportion) + MIN_SPEED;
            player.GetDamage(DamageClass.Melee) *= ((MAX_DAMAGE_BOOST - MIN_DAMAGE_BOOST) / 2 * damageProportion) + ((MAX_DAMAGE_BOOST - MIN_DAMAGE_BOOST) / 2 * healthProportion) + MIN_DAMAGE_BOOST;

            Technique.cost += 30 * ((damageProportion + healthProportion) / 2);
            base.Update(player, ref buffIndex);
        }
    }
}
