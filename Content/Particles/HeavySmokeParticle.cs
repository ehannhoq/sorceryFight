using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Particles
{
    public class HeavySmokeParticle : Particle
    {
        public override bool SetLifetime => true;
        public override int FrameVariants => 7;
        public override bool UseCustomDraw => true;
        public override bool Important => StrongVisual;
        public override bool UseAdditiveBlend => Glowing;
        public override bool UseHalfTransparency => !Glowing;

        public override string Texture => "sorceryFight/Content/Particles/HeavySmoke";

        private float Opacity;
        private float Spin;
        private bool StrongVisual;
        private bool Glowing;
        private float HueShift;
        static int FrameAmount = 6;

        public HeavySmokeParticle(Vector2 position, Vector2 velocity, Color color, int lifetime, float scale, float opacity, float rotationSpeed = 0f, bool glowing = false, float hueshift = 0f, bool required = false, bool affectedByLight = false)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            Scale = scale;
            Variant = Main.rand.Next(7);
            Lifetime = lifetime;
            Opacity = opacity;
            Spin = rotationSpeed;
            StrongVisual = required;
            Glowing = glowing;
            HueShift = hueshift;
            AffectedByLight = affectedByLight;
        }

        public override void Update()
        {
            if (Time / (float)Lifetime < 0.2f)
                Scale += 0.01f;
            else
                Scale *= 0.975f;

            Color = Main.hslToRgb((Main.rgbToHsl(Color).X + HueShift) % 1, Main.rgbToHsl(Color).Y, Main.rgbToHsl(Color).Z);
            Opacity *= 0.98f;
            Rotation += Spin * ((Velocity.X > 0) ? 1f : -1f);
            Velocity *= 0.85f;

            float opacity = Utils.GetLerpValue(1f, 0.85f, LifetimeCompletion, true);
            Color *= opacity;
        }

        public override void CustomDraw(SpriteBatch spriteBatch)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int animationFrame = (int)Math.Floor(Time / ((float)(Lifetime / (float)FrameAmount)));
            Rectangle frame = new Rectangle(80 * Variant, 80 * animationFrame, 80, 80);

            Color col = Color * Opacity;

            if (AffectedByLight)
            {
                col = col.MultiplyRGBA(Lighting.GetColor((Position / 16).ToPoint()));
            }

            spriteBatch.Draw(tex, Position - Main.screenPosition, frame, col, Rotation, frame.Size() / 2f, Scale, SpriteEffects.None, 0);
        }

    }
}
