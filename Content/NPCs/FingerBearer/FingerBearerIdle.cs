using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearerDefaultState : IAIState
    {
        private static int FRAMES = 6;
        private static int TICK_PER_FRAME = 30;
        private static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearer", AssetRequestMode.ImmediateLoad).Value;
        private static readonly float MINIMUM_DISTANCE_TO_PLAYER = 600f;

        public int frame;
        public int frametime;

        public void AI(NPC npc)
        {
            if (frametime++ >= TICK_PER_FRAME - 1)
            {
                frametime = 0;
                if (frame++ >= FRAMES - 1)
                    frame = 0;
            }

            int whoAmI = npc.FindClosestPlayer(out float distanceToPlayer);
            if (distanceToPlayer > MINIMUM_DISTANCE_TO_PLAYER) return;
            npc.target = whoAmI;
        }

        public void OnEnter(NPC npc)
        {
            frame = 0;
            frametime = 0;
        }

        public void OnExit(NPC npc)
        {
        }

        public bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = texture.Height / FRAMES;
            Rectangle src = new Rectangle(0,  * frameHeight, texture.Width, frameHeight);

            SpriteEffects spriteEffects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, src, drawColor, npc.rotation, src.Size() * 0.5f, npc.scale * 2f, spriteEffects, 0f);
            return false;
        }
    }
}