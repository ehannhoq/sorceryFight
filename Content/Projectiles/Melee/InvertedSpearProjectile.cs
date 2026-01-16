using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;


namespace sorceryFight.Content.Projectiles.Melee
{
    public class InvertedSpearProjectile : ModProjectile
    {
        public static Texture2D swordTexture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Melee/InvertedSpearProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D chainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Melee/InvertedSpearChain", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        ref float maxBounces => ref Projectile.ai[0];
        internal List<Vector2> bouncePositions;

        public override void SetDefaults()
        {
            Projectile.width = 35;
            Projectile.height = 35;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 15;
            Projectile.frameCounter = 0;

            bouncePositions = new List<Vector2>();
        }

        public override void AI()
        {
            bouncePositions[0] = Main.player[Projectile.owner].Center;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bouncePositions.Count > maxBounces)
            {
                Projectile.Kill();
                return true;
            }

            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            bouncePositions.Add(Projectile.Center);
            SoundEngine.PlaySound(SorceryFightSounds.InvertedSpearOfHeavenCollide, Projectile.Center);

            for (int i = 0; i < 7; i++)
            {
                Vector2 velocity =
                    oldVelocity +
                    Main.rand.NextVector2Circular(7f, 7f);

                int colVariation = Main.rand.Next(-38, 100);

                Color color = new Color(
                    Utils.Clamp(225 + colVariation, 0, 255),
                    Utils.Clamp(242 + colVariation, 0, 255),
                    Utils.Clamp(97 + colVariation, 0, 255)
                );

                LinearParticle particle =
                    new LinearParticle(Projectile.Center, velocity, color, false, 0.95f);

                ParticleController.SpawnParticle(particle);
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle src = swordTexture.Bounds;

            Vector2 origin = src.Size() * 0.5f;

            Main.spriteBatch.Draw(swordTexture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.rotation + MathHelper.PiOver4, origin, 2f, SpriteEffects.None, 0f);

            DrawChains(
                bouncePositions.Last(),
                Projectile.Center
            );

            if (bouncePositions.Count < 2) return false;

            for (int i = 0; i < bouncePositions.Count - 1; i++)
            {
                Vector2 firstPos = bouncePositions[i];
                Vector2 secondPos = bouncePositions[i + 1];

                DrawChains(
                    firstPos,
                    secondPos
                );
            }

            return false;
        }

        private void DrawChains(Vector2 firstPos, Vector2 secondPos)
        {
            Rectangle chainSrc = chainTexture.Bounds;
            Vector2 chainOrigin = chainSrc.Size() * 0.5f;

            Vector2 difference = secondPos - firstPos;

            float distance = difference.Length();
            float rotation = difference.ToRotation();
            Vector2 direction = difference.SafeNormalize(Vector2.UnitX);

            float scale = 2f;
            float chainLength = chainTexture.Width * scale;

            for (float increment = 0; increment < distance; increment += chainLength)
            {
                Main.spriteBatch.Draw(chainTexture, firstPos + (direction * increment) - Main.screenPosition, chainSrc, Color.White, rotation + MathHelper.PiOver4, chainOrigin, scale, SpriteEffects.None, 0f);
            }
        }


        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(bouncePositions.Count);
            foreach (Vector2 pos in bouncePositions)
            {
                writer.Write(pos.X);
                writer.Write(pos.Y);
            }
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            int num = reader.ReadInt32();

            for (int i = 0; i < num; i++)
            {
                bouncePositions.Add(
                    new Vector2(
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    )
                );
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SorceryFightSounds.InvertedSpearOfHeavenImpact, Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                int colVariation = Main.rand.Next(-38, 100);
                float scale = Main.rand.NextFloat(1f, 1.25f);
                float scalar = Main.rand.NextFloat(5f, 15f);
                SparkParticle particle = new SparkParticle(target.Center, (Projectile.velocity * scalar) + veloVariation, false, 30, scale, new Color(225 + colVariation, 242 + colVariation, 97 + colVariation));
                GeneralParticleHandler.SpawnParticle(particle);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                int colVariation = Main.rand.Next(-38, 100);
                float scale = Main.rand.NextFloat(1f, 1.25f);
                float scalar = Main.rand.NextFloat(5f, 15f);
                LineParticle particle = new LineParticle(target.Center, (Projectile.velocity * scalar) + veloVariation, false, 30, scale, new Color(225 + colVariation, 242 + colVariation, 97 + colVariation));
                GeneralParticleHandler.SpawnParticle(particle);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 posVariation = new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));
                SparkleParticle particle = new SparkleParticle(target.Center + posVariation, Vector2.Zero, new Color(225, 242, 97), Color.White, 1f, 10, 0.75f, 0.2f);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }
    }
}
