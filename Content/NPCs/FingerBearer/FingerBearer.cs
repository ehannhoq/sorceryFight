using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    [Autoload(true)]
    public class FingerBearer : BossNPC
    {
        public static readonly float MINIMUM_DISTANCE_TO_PLAYER = 900f;
        public Vector2 closestTargetPos;
        public Vector2 furthestTargetPos;

        public bool readyForOrb = true;
        private int runawayDashCooldown = 600;
        public bool onRunawayDashCooldown = false;
        public bool isSolarEclipse = false;

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 45;
            NPC.height = 86;
            NPC.npcSlots = 6;
            NPC.defense = 12;
            NPC.damage = 60;
            NPC.netAlways = true;
            NPC.aiStyle = 0;
            NPC.lifeMax = 2000;
            NPC.knockBackResist = 0.5f;
            NPC.Hitbox = new Rectangle(0, 0, NPC.width, NPC.height);
            currentState = new FingerBearerDefaultState(this);
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.eclipse)
            {
                isSolarEclipse = true;
                NPC.defense *= 2;
                NPC.damage = 180;
                NPC.lifeMax = 5000;
                runawayDashCooldown = 300;
            }
        }


        public override void AI()
        {
            int whoAmI = NPC.FindClosestPlayer(out float distanceToPlayer);
            NPC.target = distanceToPlayer <= MINIMUM_DISTANCE_TO_PLAYER ? whoAmI : -1;

            if (NPC.target == -1)
            {
                if (currentState is not FingerBearerChargeUp)
                    SetState(new FingerBearerDefaultState(this));
            }
            else
            {
                CalculateTargetPosition();

                if (CanAttack())
                    SetState(new FingerBearerPunch(this));
            }

            if (GetHealthPercentage() < 0.25 || isSolarEclipse)
            {
                if (GetDistanceToTarget() > FingerBearer.MINIMUM_DISTANCE_TO_PLAYER - 200 && readyForOrb)
                {
                    SetState(new FingerBearerChargeUp(this));
                }
                else if (GetDistanceToTarget() > FingerBearer.MINIMUM_DISTANCE_TO_PLAYER - 300 && currentState is not FingerBearerChargeUp)
                {
                    if (!onRunawayDashCooldown)
                    {
                        SetState(new FingerBearerDashState(this, furthestTargetPos));
                        onRunawayDashCooldown = TaskScheduler.Instance.AddDelayedTask(() => onRunawayDashCooldown = false, runawayDashCooldown);
                    }
                }
            }

            currentState?.AI(NPC);
        }

        private void CalculateTargetPosition()
        {
            Vector2 targetPosCenter = GetTarget().Center - new Vector2(0.0f, Math.Abs(GetTarget().height - NPC.height) / 2f);

            float heightDifference = Math.Abs(NPC.height - GetTarget().height);

            Vector2 targetPosLeft = new Vector2(targetPosCenter.X + NPC.width + 10f, targetPosCenter.Y - heightDifference / 4f);
            Vector2 targetPosRight = new Vector2(targetPosCenter.X - NPC.width - 10f, targetPosCenter.Y - heightDifference / 4f);

            float distanceFromLeft = (NPC.Center - targetPosLeft).Length();
            float distanceFromRight = (NPC.Center - targetPosRight).Length();

            if (distanceFromLeft < distanceFromRight)
            {
                closestTargetPos = targetPosLeft;
                furthestTargetPos = targetPosRight;
            }
            else
            {
                closestTargetPos = targetPosRight;
                furthestTargetPos = targetPosLeft;
            }
        }

        private bool CanAttack()
        {
            if (GetDistanceToTarget(closestTargetPos) >= 15f)
                return false;

            if (currentState is FingerBearerPunch)
                return false;

            if (currentState is FingerBearerDashState)
                return false;

            if (currentState is FingerBearerChargeUp)
                return false;

            return true;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.bloodMoon && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight && Main.hardMode)
                return 0.03f;

            if (Main.eclipse && spawnInfo.Player.ZoneOverworldHeight && NPC.downedPlantBoss)
                return 0.06f;

            return 0f;
        }
    }
}
