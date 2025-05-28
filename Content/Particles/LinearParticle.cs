using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Particles
{
    public class LinearParticle : Particle
    {
        public static Texture2D Texture => ModContent.Request<Texture2D>("sorceryFight/Content/Particles/LinearParticle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public Vector2 scaleVec;
        public LinearParticle(Vector2 position, Vector2 velocity, Color color, bool isUIParticle = false, float drag = 1, float scale = 1, int lifetime = 60) : base(Texture, position, velocity, color, isUIParticle, drag, scale, lifetime)
        {
            scaleVec = new Vector2(scale * 1.25f, scale * 0.75f);
        }

        public override void Update()
        {
            base.Update();
            scaleVec *= new Vector2(0.98f, 0.95f);

            if (scaleVec.Y < 0.01f)
                scaleVec *= new Vector2(1f, 0f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle src = new Rectangle(0, 0, Texture.Width, Texture.Height);
            Vector2 origin = src.Size() * 0.5f;


            spriteBatch.Draw(Texture, isUIParticle ? position : position - Main.screenPosition, src, color, rotation, origin, scaleVec, SpriteEffects.None, 0f);
            spriteBatch.Draw(Texture, isUIParticle ? position : position - Main.screenPosition, src, new Color(255, 255, 255, 160), rotation, origin, scaleVec * 0.90f, SpriteEffects.None, 0f);

        }
    }
}

