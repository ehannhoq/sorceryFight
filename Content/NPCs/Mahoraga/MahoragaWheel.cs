using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.Mahoraga
{
    internal class MahoragaWheel(MahoragaBoss mahoraga)
    {
        private static Texture2D wheelTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Mahoraga/MahoragaWheel", AssetRequestMode.ImmediateLoad).Value;
        
        private bool adapting;
        private int wheelFrame = 0;
        private int wheelFrameTime = 0;
        private const int WHEEL_FRAMES = 4;
        private const int WHEEL_TICKS_PER_FRAME = 8;

        public void Update()
        {
            if (!adapting)
            {
                wheelFrame = 0;
                wheelFrameTime = 0;
                return;
            }

            if (wheelFrameTime++ >= WHEEL_TICKS_PER_FRAME)
            {
                wheelFrameTime = 0;
                if (wheelFrame++ >= WHEEL_FRAMES - 1)
                {
                    wheelFrame = 0;
                    adapting = false;
                    SoundEngine.PlaySound(SorceryFightSounds.MahoragaAdaptation);
                }
            }
        }

        public void Spin()
        {
            adapting = true;
        }

        public void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Vector2 wheelPosition = mahoraga.NPC.Center - new Vector2(0.0f, mahoraga.NPC.height / 2) - new Vector2(20.0f * -mahoraga.NPC.direction, 15.0f);

            int frameHeight = wheelTexture.Height / WHEEL_FRAMES;
            int frameY = wheelFrame * frameHeight;
            Rectangle src = new Rectangle(0, frameY, wheelTexture.Width, frameHeight);
            SpriteEffects spriteEffects = mahoraga.NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(wheelTexture, wheelPosition - Main.screenPosition, src, drawColor, mahoraga.NPC.rotation, src.Size() * 0.5f, mahoraga.NPC.scale * 2f, spriteEffects, 0f);
        }
    }
}