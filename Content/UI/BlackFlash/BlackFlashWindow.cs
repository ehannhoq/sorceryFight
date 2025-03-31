using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class BlackFlashWindow : UIElement
    {
        private int timer;
        private int lowerBound;
        private int upperBound;
        private Texture2D borderTexture;
        private Texture2D pointer;

        public BlackFlashWindow(int lowerBound, int upperBound) : base()
        {
            this.timer = 0;
            this.lowerBound = 2 * lowerBound;
            this.upperBound = 2 * upperBound;
            this.borderTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/BlackFlash/BlackFlashWindowBorder", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            this.pointer = ModContent.Request<Texture2D>("sorceryFight/Content/UI/BlackFlash/BlackFlashWindowPointer", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            timer++;

            if (timer == 30)
            {
                var par = (SorceryFightUI)Parent;
                par.RemoveElement(this);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 borderPos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2) - new Vector2(borderTexture.Width / 2, borderTexture.Height + 50);
            spriteBatch.Draw(borderTexture, borderPos / Main.UIScale, Color.White);

            Texture2D whitePixel = TextureAssets.MagicPixel.Value;

            Rectangle bgBarSrcRect = new Rectangle((int)borderPos.X + 3, (int)borderPos.Y + 3, borderTexture.Width - 6, borderTexture.Height - 6);
            spriteBatch.Draw(whitePixel, bgBarSrcRect, Color.Black);

            int windowWidth = Math.Abs(upperBound - lowerBound);

            Vector2 windowBarPos = new Vector2(borderPos.X + 3 + (bgBarSrcRect.Width - upperBound), borderPos.Y + 3);
            Rectangle windowSrcRect = new Rectangle((int)windowBarPos.X, (int)windowBarPos.Y, windowWidth, bgBarSrcRect.Height);

            spriteBatch.Draw(whitePixel, windowSrcRect, Color.Red);

            Vector2 pointerPos = new Vector2(borderPos.X + 3 + ((bgBarSrcRect.Width * 2) * ((float)timer / bgBarSrcRect.Width)), borderPos.Y + (borderTexture.Height / 2) - (pointer.Height / 2));
            spriteBatch.Draw(pointer, pointerPos / Main.UIScale, Color.White);
        }
    }
}