using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Utilities.EaseFunctions;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Particles
{
    public class StarParticle : Particle
    {
        public static Texture2D Texture = ModContent.Request<Texture2D>("sorceryFight/Content/Particles/StarParticle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public float originalScale;
        public bool changeOpacity;
        public bool changeScale;
        public float opacity = 1f;
        public StarParticle(Vector2 position, Vector2 velocity,  Color color, float rotation = 0f, bool changeOpacity = false, bool changeScale = false, bool isUIParticle = false, float drag = 1, float scale = 1, int lifetime = 60) : base(Texture, position, velocity, color, isUIParticle, drag, scale, lifetime)
        {
            originalScale = scale;
            this.changeOpacity = changeOpacity;
            this.changeScale = changeScale;

            if (velocity == Vector2.Zero)
                this.rotation = rotation;
        }

        public override void Update()
        {
            base.Update();
            float progress = (float)time / lifetime;

            if (changeScale)
                scale = float.Lerp(originalScale, 0f, EaseFunctions.EaseInCircular(progress));

            if (changeOpacity)
                opacity = float.Lerp(opacity, 0f, EaseFunctions.EaseInCircular(progress));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle src = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = src.Size() * 0.5f;

            spriteBatch.Draw(Texture, isUIParticle ? position : position - Main.screenPosition, src, new Color(color.R, color.G, color.B, (int)(opacity * 255)), rotation, origin, scale, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture, isUIParticle ? position : position - Main.screenPosition, src, new Color(255, 255, 255, (int)(opacity * 255)), rotation, origin, scale * 0.5f, SpriteEffects.None, 0f);
        }
    }
}

