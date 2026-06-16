using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    [Autoload(true)]
    public class FingerBearer : BossNPC
    {
        public static readonly float MINIMUM_DISTANCE_TO_PLAYER = 700f;
        public Vector2 closestTargetPos;
        public Vector2 furthestTargetPos;

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

        public override void AI()
        {
            int whoAmI = NPC.FindClosestPlayer(out float distanceToPlayer);
            NPC.target = distanceToPlayer <= MINIMUM_DISTANCE_TO_PLAYER ? whoAmI : -1;

            float distanceToTarget = Math.Min((NPC.Center - closestTargetPos).Length(), (NPC.Center - furthestTargetPos).Length());

            if (NPC.target == -1)
            {
                SetState(new FingerBearerDefaultState(this));
            }
            else
            {
                CalculateTargetPosition();

                if (distanceToTarget < 15f && currentState is not FingerBearerPunch)
                    SetState(new FingerBearerPunch(this));
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
    }
}