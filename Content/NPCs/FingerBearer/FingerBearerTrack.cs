using System;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearerTrack(BossNPC bossNPC) : AIState(bossNPC)
    {
        private static readonly int FRAMES = 7;
        private static readonly int TICK_PER_FRAME = 7;
        private static readonly Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearerWalk", AssetRequestMode.ImmediateLoad).Value;
        public static float movementSpeed = 4f;

        public int frame;
        public int frametime;

        public override void AI(NPC npc)
        {
            if (frametime++ >= TICK_PER_FRAME - 1)
            {
                frametime = 0;
                if (frame++ >= FRAMES - 1)
                    frame = 0;
            }


            Vector2 toPlayer = npc.Center.DirectionTo(((FingerBearer)bossNPC).closestTargetPos);
            Vector2 xVector = Vector2.UnitX * npc.direction;

            float dotprod = Vector2.Dot(toPlayer, xVector);
            Vector2 projection = dotprod / xVector.LengthSquared() * xVector;

            npc.velocity.X = (projection * movementSpeed).X;

            DashIfPlayerRunningAway(npc);
            // DashIfPlayerAbove(npc);
        }

        private void DashIfPlayerRunningAway(NPC npc)
        {
            Player player = Main.player[npc.target];
            Vector2 playerVelocity = player.velocity;
            Vector2 toNPC = player.Center - npc.Center;

            playerVelocity.Normalize();
            toNPC.Normalize();
            float playerRelativeVelocity = Vector2.Dot(playerVelocity, toNPC);

            if (playerRelativeVelocity > 0.75f && player.velocity.Length() > 12f) // player is most likely running away from finger bearer
            {
                FingerBearer fingerBearer = (FingerBearer) bossNPC;
                Vector2 dashPos = fingerBearer.furthestTargetPos + player.velocity.SafeNormalize(Vector2.Zero) * (npc.width + 10f) * 15f;
                bossNPC.SetState(new FingerBearerDashState(bossNPC, dashPos));
                Main.NewText("run");
            }
        }


        private void DashIfPlayerAbove(NPC npc)
        {
            Player player = Main.player[npc.target];
            FingerBearer fingerBearer = (FingerBearer) bossNPC;
            float yDiff = Math.Abs(player.Center.Y - npc.Center.Y);
            if (yDiff > 30f)
            {
                bool coinFlip = Main.rand.NextFloat() > 0.5;
                Vector2 dashPos = coinFlip ? fingerBearer.closestTargetPos : fingerBearer.furthestTargetPos;
                bossNPC.SetState(new FingerBearerDashState(bossNPC, dashPos));
                Main.NewText("above");
            }
        }


        public override void OnEnter(NPC npc)
        {
            frame = 0;
            frametime = 0;
        }

        public override void OnExit(NPC npc)
        {
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = texture.Height / FRAMES;
            Rectangle src = new Rectangle(0, frame * frameHeight, texture.Width, frameHeight);

            SpriteEffects spriteEffects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, src, drawColor, npc.rotation, src.Size() * 0.5f, npc.scale * 2f, spriteEffects, 0f);
            return false;
        }
    }
}