using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class MaximumOutputBlue : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8; 
        public static readonly int TICKS_PER_FRAME = 5;

        public override string Name { get; set; } = "Maximum Cursed Energy Output: Blue";
        public override float Cost { get; set; } = -1f;
        public override float CostPercentage { get; set; } = 30f;
        public override float MasteryNeeded { get; set; } = 0f;
        public override Color textColor { get; set; } = new Color(108, 158, 240);

        public override int Damage { get; set ; } = 150;
        public override float Speed { get; set; } = 20f;
        public override float LifeTime { get; set; } = 300f;
        public NPC lockOnTarget;

        public static Texture2D texture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<MaximumOutputBlue>();
        } 

        public override float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = true;
            Projectile.penetrate = 10;
        }
        public override void AI()
        {
            Projectile.ai[0] += 1;

            if (Projectile.ai[0] > LifeTime)
            {
                Projectile.Kill();
            }

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            if (lockOnTarget != null)
            {
                Projectile.Center = lockOnTarget.Center;
                Projectile.scale -= -0.1f;
            }
            else if (Projectile.ai[0] <= 30)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 mousePos = Main.MouseWorld;
                    Projectile.ai[1] = mousePos.X;
                    Projectile.ai[2] = mousePos.Y;
                    Projectile.netUpdate = true;
                }

                Vector2 targetPos = new Vector2(Projectile.ai[1], Projectile.ai[2]);
                Vector2 direction = targetPos - Projectile.Center;

                Projectile.velocity = direction.SafeNormalize(Vector2.Zero * Projectile.velocity.Length()) * Speed;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/MaximumOutputBlue").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 0.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lockOnTarget = target; 
        }
    }
}