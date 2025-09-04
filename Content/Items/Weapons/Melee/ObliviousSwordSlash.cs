using System;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using sorceryFight.Content.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class ObliviousSwordSlash : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/CursedTechniques/CursedTechnique";
        private Texture2D texture;
        private const int FRAME_COUNT = 60;


        private int SwingPhase => Projectile.frame / 20;

        public override void SetDefaults()
        {
            Projectile.width = 550;
            Projectile.height = 550;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 20;
        }

        public override void AI()
        {
            if ((Projectile.frame += 1) >= FRAME_COUNT - 1)
            {
                Projectile.frame = 0;
            }

            if (Projectile.frame % 20 == 10)
            {

                if (Main.myPlayer == Projectile.owner)
                {
                    if (SwingPhase == 0)
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            Vector2 velocity = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.Pi / 8);
                            velocity = velocity.RotatedBy(-MathHelper.Pi / 8 * i);

                            Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center, velocity * 20f, ModContent.ProjectileType<ObliviousSwordProjectile>(), Projectile.damage / 2, 0f, Projectile.owner);

                        }
                    }
                    else
                    {
                        Projectile.NewProjectile(Main.player[Projectile.owner].GetSource_FromThis(), Projectile.Center, Projectile.velocity * 20f, ModContent.ProjectileType<ObliviousSwordProjectile>(), Projectile.damage / 3, 0f, Projectile.owner);
                    }


                    SoundEngine.PlaySound(SwingPhase == 0 ? SorceryFightSounds.OblivionSwordBigSlash : SorceryFightSounds.OblivionSwordSlash, Projectile.Center);

                }
            }

            Player player = Main.player[Projectile.owner];
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            float velocityAngle = Projectile.velocity.ToRotation();
            float offset = 30f * Projectile.scale;

            Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
            player.ChangeDir(Projectile.direction);

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.CantUseSword(Projectile))
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
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

            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Items/Weapons/Melee/ObliviousSwordSlash/{Projectile.frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, -32).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation, projOrigin, 1f, spriteEffects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SwingPhase == 0 ? SorceryFightSounds.OblivionSwordBigImpact : SorceryFightSounds.OblivionSwordImpact, target.Center);

            for (int i = 0; i < 10; i++)
            {
                Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                int colVariation = Main.rand.Next(-38, 100);
                float scale = Main.rand.NextFloat(0.9f, 1.2f);
                LinearParticle particle = new LinearParticle(target.Center, (Projectile.velocity * 35) + veloVariation, new Color(252 + colVariation, 232 + colVariation, 151 + colVariation), default, 0.9f, scale, 45);
                ParticleController.SpawnParticle(particle);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 posVariation = new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));
                SparkleParticle particle = new SparkleParticle(target.Center + posVariation, Projectile.velocity * 2f, new Color(252, 232, 151), Color.White, 2f, 10, 0.75f, 0.2f);
                GeneralParticleHandler.SpawnParticle(particle);
            }

            if (SwingPhase == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                    int colVariation = Main.rand.Next(-38, 100);
                    float scale = Main.rand.NextFloat(1.5f, 2f);
                    LinearParticle particle = new LinearParticle(target.Center, (Projectile.velocity * 35) + veloVariation, new Color(240 + colVariation, 92 + colVariation, 79 + colVariation), default, 0.9f, scale, 45);
                    ParticleController.SpawnParticle(particle);
                }
            }
        }
    }
}
