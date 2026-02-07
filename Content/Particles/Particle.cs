using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace sorceryFight.Content.Particles
{
    public abstract class Particle
    {
        internal Texture2D texture;
        internal Vector2 position;
        internal Vector2 velocity;
        internal float drag;
        internal float scale;
        internal Color color;
        internal int lifetime;
        internal float rotation;

        internal bool isUIParticle;

        internal int time;


        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, Color color, bool isUIParticle = false, float drag = 1f, float scale = 1f, int lifetime = 60)
        {
            this.texture = texture;
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.isUIParticle = isUIParticle;
            this.drag = drag;
            this.scale = scale;
            this.lifetime = lifetime;

            rotation = position.DirectionTo(position + velocity).ToRotation();
        }

        public virtual void Update()
        {
            position += velocity;
            velocity *= drag;

            // Main.NewText($"Pos: {position} Vel: {velocity} Drag: {drag} Scale: {scale} Color: {color} Lifetime: {lifetime} Rotation: {rotation}");

            time++;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
            spriteBatch.Draw(texture, isUIParticle ? position : position - Main.screenPosition, src, color, rotation, src.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}