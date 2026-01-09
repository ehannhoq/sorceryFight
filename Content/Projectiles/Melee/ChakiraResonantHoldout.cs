using System;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using sorceryFight.Content.Projectiles;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.Melee
{
    public class ChakiraResonantHoldout : ModProjectile
    {
        private static readonly float MIN_CHARGE = 60;
        private static readonly float MAX_CHARGE = 300 + MIN_CHARGE;
        public override string Texture => "sorceryFight/Content/CursedTechniques/CursedTechnique";
        private Texture2D texture;
        private const int FRAME_COUNT = 48;

        private bool IsSlash => Projectile.ai[0] == 0f;
        private ref float chargeProjIndex => ref Projectile.ai[1];
        private ref float charge => ref Projectile.ai[2];

        public override void OnSpawn(IEntitySource source)
        {
            chargeProjIndex = -1f;
        }

        public override void SetDefaults()
        {
            Projectile.width = 550;
            Projectile.height = 550;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 25;
        }


        public override void AI()
        {
            if ((Projectile.frame += 1) >= FRAME_COUNT - 1)
            {
                Projectile.frame = 0;
            }

            HandleProjectileHoldout();

            if (IsSlash && (Projectile.frame == 8 || Projectile.frame == 30))
                SoundEngine.PlaySound(SorceryFightSounds.OblivionSwordSlash with { PitchVariance = 0.15f, Pitch = 0.15f }, Projectile.Center);

            if (!IsSlash && Projectile.active)
            {
                if (charge == 0)
                {
                    SoundEngine.PlaySound(SorceryFightSounds.ChakiraResonantChargeUp, Projectile.Center);
                }

                SorceryFightPlayer sfPlayer = Main.player[Projectile.owner].SorceryFight();
                if (sfPlayer.cursedEnergy > 1 && charge < MAX_CHARGE)
                {
                    sfPlayer.disableRegenFromProjectiles = true;

                    float cursedEnergyPerTick = (sfPlayer.maxCursedEnergy - 100) / MAX_CHARGE;
                    sfPlayer.cursedEnergy -= cursedEnergyPerTick;

                    if (charge++ >= MAX_CHARGE)
                        charge = MAX_CHARGE;
                }

                if (charge < MIN_CHARGE)
                    return;


                float prog = (charge - MIN_CHARGE) / MAX_CHARGE;
                float logProg = MathF.Log10(prog + 0.01f) + 2f;
                logProg /= 2f;

                if (Main.myPlayer == Projectile.owner)
                {
                    if (chargeProjIndex == -1f)
                    {
                        Vector2 spawnPos = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 350f;
                        chargeProjIndex = Projectile.NewProjectile(sfPlayer.Player.GetSource_FromThis(), spawnPos, Vector2.Zero, ModContent.ProjectileType<ChakiraResonantCharge>(), 0, 0, Projectile.owner, Projectile.whoAmI);
                    }

                    float cameraZoom = -(logProg / 4);
                    CameraController.SetCameraZoom(cameraZoom);

                    Vector2 cameraOffset = Main.rand.NextVector2CircularEdge(prog * 7f, prog * 7f);
                    CameraController.SetCameraPosition(sfPlayer.Player.Center + cameraOffset);
                }

                Projectile chargeProj = Main.projectile[(int)chargeProjIndex];
                chargeProj.ai[0] = logProg;
                chargeProj.Center = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitX) * 350f;
            }
        }

        private void HandleProjectileHoldout()
        {
            float offset = IsSlash ? 30f : 200f;

            Player player = Main.player[Projectile.owner];
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            float velocityAngle = Projectile.velocity.ToRotation();

            Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
            player.ChangeDir(Projectile.direction);

            if (Main.myPlayer == Projectile.owner)
            {
                if (!IsSlash)
                    player.channel = PlayerInput.Triggers.Current.MouseRight;

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

            string currentPath = IsSlash ? "ChakiraResonantSlash" : "ChakiraResonantHoldout";

            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Projectiles/Melee/{currentPath}/{Projectile.frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float scale = IsSlash ? 1f : 0.66f;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, -32).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation, projOrigin, scale, spriteEffects, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SorceryFightSounds.OblivionSwordImpact with { PitchVariance = 0.25f }, target.Center);

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
        }

        public override void OnKill(int timeLeft)
        {
            if (!IsSlash && chargeProjIndex != -1f)
            {
                Projectile chargeProj = Main.projectile[(int)chargeProjIndex];

                float progress = charge / MAX_CHARGE;
                float minSpeed = 10f;
                float maxSpeed = 25f;

                chargeProj.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * (maxSpeed - ((maxSpeed - minSpeed) * progress));
                chargeProj.damage = Projectile.damage + (int)(Projectile.damage * 5 * progress);
                chargeProj.netUpdate = true;

                SorceryFightPlayer sfPlayer = Main.player[Projectile.owner].SorceryFight();
                sfPlayer.disableRegenFromProjectiles = false;

                CameraController.ResetCameraZoom();
                CameraController.ResetCameraPosition();

                SoundEngine.PlaySound(SorceryFightSounds.ChakiraResonantProjectileFire, Projectile.Center);
            }
        }
    }
}
