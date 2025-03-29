using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.BlackFlash
{
    public class BFImpactElement : UIElement
    {
        private const int FRAMES = 2;
        private const int TICKS_PER_FRAME = 4;
        private int frameCounter = 0;
        private int frame = 0;
        private Vector2 screenPos;
        private Texture2D texture;
        public BFImpactElement(Vector2 screenPos) : base()
        {
            this.screenPos = screenPos;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/BlackFlash/Impact_Frames", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (frameCounter++ >= TICKS_PER_FRAME)
            {
                frameCounter = 0;
                frame++;
            }

            if (frame >= FRAMES)
            {
                SorceryFightUI parent = (SorceryFightUI)Parent;
                parent.RemoveElement(this);
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Texture2D whitePixel = TextureAssets.MagicPixel.Value;
            Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            Color bgColor = frame == 0 ? Color.White : Color.Black;

            spriteBatch.Draw(whitePixel, screenRectangle, bgColor);

            int frameHeight = texture.Height / FRAMES;
            int frameY = frame * frameHeight;
            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            spriteBatch.Draw(texture, screenPos, sourceRectangle, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}

