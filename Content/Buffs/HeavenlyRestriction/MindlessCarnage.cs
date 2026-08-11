using Microsoft.Xna.Framework;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Graphics.Effects;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using sorceryFight.Utilities.EaseFunctions;
using Terraria.Graphics.Shaders;
using System;

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

        private static int tick = 0;
        private static int version = 0;

        public override void OnApply(Player player)
        {
            if (Main.myPlayer != player.whoAmI) return;

            Filters.Scene.Activate("SF:MindlessCarnage");

            int myVersion = ++version;
            
            TaskScheduler.Instance.AddContinuousTask(() => {
                if (myVersion != version) return;
                tick = System.Math.Min(tick + 1, 60);
                Filters.Scene["SF:MindlessCarnage"].GetShader().UseOpacity(EaseFunctions.EaseInCircular(tick / 60f));
            }, 60);
        }

        public override void OnRemove(Player player)
        {
            if (Main.myPlayer != player.whoAmI) return;

            int myVersion = ++version;
            
            TaskScheduler.Instance.AddContinuousTask(() => {
                if (myVersion != version) return;
                
                tick = System.Math.Max(tick - 1, 0);
                Filters.Scene["SF:MindlessCarnage"].GetShader().UseOpacity(EaseFunctions.EaseInCircular(tick / 60f));
            }, 60);

            TaskScheduler.Instance.AddDelayedTask(() => {
                if (myVersion != version) return;

                Filters.Scene["SF:MindlessCarnage"].GetShader().UseOpacity(0f);
                Filters.Scene["SF:MindlessCarnage"].Deactivate();
            }, 61);

            CameraController.ResetCameraPosition();
            CameraController.ResetCameraZoom();
        }

        public override void Update(Player player, ref int buffIndex)
        {
            Technique.cost = 65f;
            float ease = EaseFunctions.EaseInExponential(power: 3, tick / 60f);

            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 cameraOffset = new Vector2(Main.rand.NextFloat(-5 * ease, 5 * ease), Main.rand.NextFloat(-2 * ease, 2 * ease));
                CameraController.SetCameraPosition(player.Center + cameraOffset);

                Filters.Scene["SF:MindlessCarnage"].GetShader().UseTargetPosition(player.Center);
            }

            player.AddBuff(BuffID.Dangersense, 2);
            player.AddBuff(BuffID.Hunter, 2);
            player.statDefense /= 0.8f;

            float minDistance = 2000f;
            NPC nearestStrongestNPC = null;

            float npcHealth = 0;
            float npcDamage = 0;
            float npcDefense = 0;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.type == NPCID.TargetDummy || !npc.active) continue;

                float dist = (npc.Center - player.Center).Length();
                if (dist < minDistance)
                {
                    if (npcDamage < npc.damage || npcHealth < npc.lifeMax)
                    {
                        nearestStrongestNPC = npc;
                        npcHealth = npc.lifeMax;
                        npcDamage = npc.damage;
                        npcDefense = npc.defense;
                    }
                }
            }

            if (nearestStrongestNPC == null) return;

            Vector3 npcVector = new Vector3(npcHealth, npcDamage, npcDefense);
            Vector3 strongestVector = new Vector3(SorceryFightMod.strongestBoss.lifeMax, SorceryFightMod.strongestBoss.damage, SorceryFightMod.strongestBoss.defense);

            float npcLength = npcVector.Length();
            float strongestLength = strongestVector.Length();

            npcLength += 0.001f;
            strongestLength += 0.001f;

            float cosTheta = Vector3.Dot(npcVector, strongestVector) / (npcLength * strongestLength);
            float lengthDifference = MathF.Abs(npcLength - strongestLength);

            float angleSimularity = (cosTheta + 1) / 2f;
            float magnitudeSimularity = 1 - (lengthDifference / MathF.Max(npcLength, strongestLength)); 

            float simularity = angleSimularity * magnitudeSimularity;

            float speedDiff = MAX_SPEED - MIN_SPEED;
            float dmgDiff = MAX_DAMAGE_BOOST - MIN_DAMAGE_BOOST;

            player.moveSpeed += speedDiff * simularity + MIN_SPEED;
            player.GetDamage(DamageClass.Melee) *= dmgDiff * simularity + MIN_DAMAGE_BOOST;

            Technique.cost += 30 * simularity;
            base.Update(player, ref buffIndex);
        }
    }
}
