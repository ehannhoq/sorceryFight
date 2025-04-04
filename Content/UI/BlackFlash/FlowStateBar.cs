using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.BlackFlash
{
    public class FlowStateBar : UIElement
    {
        private const int FRAMES = 6;
        private const int TICKS_PER_FRAME = 3;
        private int[] icon_frame;
        private int frametime;
        private Texture2D emptyTexture;
        private Texture2D fullTexture;
        private SorceryFightPlayer sfPlayer;
        public FlowStateBar() : base()
        {
            icon_frame = [0, 3, 5];
            frametime = 0;

            emptyTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/BlackFlash/FlowStateEmptyIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            fullTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/BlackFlash/FlowStateFilledIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
        }

        public override void Update(GameTime gameTime)
        {
            if (frametime++ >= TICKS_PER_FRAME)
            {
                frametime = 0;

                for (int i = 0; i < icon_frame.Length; i++)
                {
                    if (icon_frame[i]++ >= FRAMES - 1)
                        icon_frame[i] = 0;
                }
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();

            for (int i = 0; i < icon_frame.Length; i++)
            {
                int frame = icon_frame[i];

                int frameHeight = emptyTexture.Height / FRAMES;
                int frameY = frame * frameHeight;
                Rectangle sourceRectangle = new Rectangle(0, frameY, emptyTexture.Width, frameHeight);
                Vector2 origin = new Vector2(emptyTexture.Width / 2, frameHeight / 2);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                spriteBatch.Draw(emptyTexture, new Vector2(dimensions.X, dimensions.Y) + new Vector2(0, i * (frameHeight + 5)), sourceRectangle, Color.White, 0f, origin, 0.75f * Main.UIScale, SpriteEffects.None, 0f);
                if (i < sfPlayer.blackFlashCounter - 1)
                {
                    spriteBatch.Draw(fullTexture, new Vector2(dimensions.X, dimensions.Y) + new Vector2(0, i * (frameHeight + 5)), sourceRectangle, Color.White, 0f, origin, 0.75f * Main.UIScale, SpriteEffects.None, 0f);
                }
                spriteBatch.End();
                spriteBatch.Begin();
            }
        }
    }
}
