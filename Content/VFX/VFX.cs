using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace sorceryFight.Content.VFX
{
    public class VFXObject
    {
        public Texture2D Texture { get; private set; }
        public Vector2 center;
        public VFXDrawLayer drawLayer;
        public int lifetime;
        public float scale;
        public int frames;
        public int ticksPerFrame;
        public Color color;

        public float rotation = 0f;
        public float opacity = 1f;

        internal int tick = 0;

        private int frame = 0;
        private int frameTime = 0;

        public VFXObject(Texture2D texture, Vector2 center, VFXDrawLayer drawLayer = VFXDrawLayer.AboveNPCs, int lifetime = -1, int frames = 1, int ticksPerFrame = 1, float scale = 1f, Color? color = null)
        {
            Texture = texture;
            this.center = center;
            this.drawLayer = drawLayer;
            this.lifetime = lifetime == -1 ? frames * ticksPerFrame : lifetime;
            this.frames = frames;
            this.ticksPerFrame = ticksPerFrame;
            this.scale = scale;
            this.color = color ?? Color.White;
        }

        internal virtual void Update()
        {
            if (++frameTime >= ticksPerFrame)
            {
                frameTime = 0;

                if (++frame >= frames)
                {
                    frame = 0;
                }
            }
        }

        internal virtual void Draw(SpriteBatch spriteBatch)
        {
            int frameHeight = Texture.Height / frames;
            int frameY = frame * frameHeight;

            Rectangle src = new Rectangle(0, frameY, Texture.Width, frameHeight);

            spriteBatch.Draw(Texture, center - Main.screenPosition, src, new Color(color.R, color.G, color.B, (int)(opacity * 255)), rotation, src.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}