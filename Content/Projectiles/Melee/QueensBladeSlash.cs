using System.Linq;
using JetBrains.Annotations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.Melee
{
    public class QueensBladeSlash : ModProjectile
    {
        private static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Melee/QueensBladeSlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        private const int FRAME_COUNT = 71;
        private const int FRAME_GRID_WIDTH = 8;
        private const int FRAME_GRID_HEIGHT = 9;
        private const int TICKS_PER_FRAME = 1;

        private const int BIG_SLASH_START = 40;
        private const int BIG_SLASH_END = 53;
        private bool bigSlash => Projectile.frame >= 40 && Projectile.frame <= 53;

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 180;
            Projectile.scale = 1.25f;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.PositionProjectileForSlash(20f);
        }

        public override void AI()
        {
            Projectile.timeLeft = 2;

            if (Projectile.frame == FRAME_COUNT - 1)
            {
                if (++Projectile.ai[0] >= 15.0f)
                {
                    Projectile.ai[0] = 0;
                    Projectile.frame = 0;
                }
            }
            else
                Projectile.HandleProjectileAnimation(FRAME_COUNT, TICKS_PER_FRAME);

            Projectile.PositionProjectileForSlash(20f);
        }

        public override bool? CanHitNPC(NPC target)
        {
            int[] frames = [6, 17, 28, 45];
            if (frames.Contains(Projectile.frame))
                return true;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            for (int i = 0; i < 3; i++)
            {
                StarParticle particle = new StarParticle(
                    position: target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f),
                    velocity: Vector2.Zero,
                    color: new Color(181, 159, 178),
                    changeOpacity: true,
                    lifetime: Projectile.frame == 45 ? 12 : 6,
                    scale: Projectile.frame == 45 ? 1f : 0.7f
                );
                ParticleController.SpawnParticle(particle);
            }


            if (Projectile.frame == 45 && Main.myPlayer == Projectile.owner)
            {
                Vector2 hitDir = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);



                for (int i = 0; i < 7; i++)
                {
                    LinearParticle particle = new LinearParticle(
                        position: target.Center,
                        velocity: hitDir.RotatedByRandom(0.75) * Main.rand.NextFloat(10f, 20f),
                        color: new Color(181, 159, 178),
                        lifetime: 15
                    );
                    ParticleController.SpawnParticle(particle);
                }


                float rotation = MathHelper.PiOver4 / 2f;

                Projectile.NewProjectile(
                    Main.player[Projectile.owner].GetSource_FromThis(),
                    target.Center + hitDir.RotatedBy(-rotation) * 80f,
                    Vector2.Zero,
                    ModContent.ProjectileType<RikaPunchProjectile>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    ai0: target.whoAmI
                );

                Projectile.NewProjectile(
                    Main.player[Projectile.owner].GetSource_FromThis(),
                    target.Center + hitDir.RotatedBy(rotation) * 80f,
                    Vector2.Zero,
                    ModContent.ProjectileType<RikaPunchProjectile>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    ai0: target.whoAmI
                );
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 center = Main.player[Projectile.owner].Center;
            Vector2 end = center + Projectile.velocity * Projectile.width;
            float lineWidth = Projectile.height - (bigSlash ? 0 : 40);
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, end, lineWidth, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameWidth = texture.Width / FRAME_GRID_WIDTH;
            int frameHeight = texture.Height / FRAME_GRID_HEIGHT;

            int frameX = Projectile.frame / FRAME_GRID_HEIGHT * frameWidth;
            int frameY = Projectile.frame % FRAME_GRID_HEIGHT * frameHeight;

            Rectangle sourceRectangle = new Rectangle(frameX, frameY, frameWidth, frameHeight);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, projOrigin, Projectile.scale, spriteEffects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                Main.DefaultSamplerState,
                DepthStencilState.None,
                RasterizerState.CullCounterClockwise,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            return false;
        }
    }
}
