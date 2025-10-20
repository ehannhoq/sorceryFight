using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using CalamityMod.NPCs.Providence;
using sorceryFight.Content.Items.Accessories;
using System;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.CeaselessVoid;
using sorceryFight.Content.Projectiles.VFX;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class HollowPurple : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;
        private static readonly float COLLISION_TIME = HollowPurpleCollision.FRAMES * HollowPurpleCollision.TICKS_PER_FRAME;
        private static readonly float WAIT_TIME = 110f;

        public static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/HollowPurple", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D prism1 = ModContent.Request<Texture2D>("sorceryFight/Content/VFXSprites/HollowPurplePrisms1", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D prism2 = ModContent.Request<Texture2D>("sorceryFight/Content/VFXSprites/HollowPurplePrisms2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D prism3 = ModContent.Request<Texture2D>("sorceryFight/Content/VFXSprites/HollowPurplePrisms3", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D ring = ModContent.Request<Texture2D>("sorceryFight/Content/VFXSprites/HollowPurpleRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static Texture2D flash = ModContent.Request<Texture2D>("sorceryFight/Content/VFXSprites/WhiteFlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.HollowPurple.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.HollowPurple.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.HollowPurple.LockedDescription");
        public override float Cost { get; } = 525f;
        public override Color textColor { get; } = new Color(235, 117, 233);
        public override bool DisplayNameInGame { get; } = true;
        public override int Damage => 13000;
        public override int MasteryDamageMultiplier => 450;
        public override float Speed { get; } = 45f;
        public override float LifeTime { get; } = 500f;

        public Projectile collisionVFX;
        public int glareIndex;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<Signus>()) && sf.HasDefeatedBoss(ModContent.NPCType<StormWeaverHead>()) && sf.HasDefeatedBoss(ModContent.NPCType<CeaselessVoid>());
        }
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<HollowPurple>();
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 75;
            Projectile.height = 75;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            collisionVFX = null;
            glareIndex = 0;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            Vector2 projOrigin = player.MountedCenter + player.MountedCenter.DirectionTo(Main.MouseWorld) * 50f;

            if (Projectile.ai[0] < COLLISION_TIME)
            {
                if (collisionVFX == null)
                {
                    int index = Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<HollowPurpleCollision>(), 0, 0, player.whoAmI);
                    collisionVFX = Main.projectile[index];
                    SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleCollide, Projectile.Center);
                }

                Projectile.Center = projOrigin;
                Projectile.velocity = Vector2.Zero;
                Projectile.damage = 0;
                collisionVFX.Center = projOrigin;
                Projectile.netUpdate = true;
                sfPlayer.disableRegenFromProjectiles = true;

                if (Projectile.ai[0] == COLLISION_TIME - 20)
                    SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleShine, Projectile.Center);

            }
            else if (Projectile.ai[0] < COLLISION_TIME + WAIT_TIME)
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.Center = projOrigin;

                Projectile.netUpdate = true;

            }
            else if (Projectile.ai[0] == COLLISION_TIME + WAIT_TIME)
            {
                Projectile.damage = (int)CalculateTrueDamage(player.GetModPlayer<SorceryFightPlayer>());

                Projectile.timeLeft = (int)LifeTime;
                Projectile.Center = projOrigin;

                if (Main.myPlayer == Projectile.owner)
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;

                sfPlayer.disableRegenFromProjectiles = false;
                Projectile.netUpdate = true;
                SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleRelease, Projectile.Center);

                for (int i = 0; i < 30f; i++)
                {
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                    float angleOffset = MathHelper.ToRadians(Main.rand.NextFloat(-30f, 30f));
                    direction = direction.RotatedBy(angleOffset);

                    float speed = Main.rand.NextFloat(50f, 90f);

                    Vector2 velocity = direction * speed;

                    LinearParticle particle = new LinearParticle(Projectile.Center, velocity, textColor, false, 0.9f, 2f);
                    ParticleController.SpawnParticle(particle);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            if (Projectile.ai[0] > COLLISION_TIME)
            {
                if (Projectile.ai[0] < COLLISION_TIME + WAIT_TIME)
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

                    Rectangle prismSrc = new Rectangle(0, 0, prism1.Width, prism1.Height);
                    Main.EntitySpriteDraw(prism1, Projectile.Center - Main.screenPosition, prismSrc, Color.White, Projectile.ai[0] / 10, prismSrc.Size() * 0.5f, 1f, SpriteEffects.None);
                    Main.EntitySpriteDraw(prism2, Projectile.Center - Main.screenPosition, prismSrc, Color.White, Projectile.ai[0] / -20f, prismSrc.Size() * 0.5f, 1.5f, SpriteEffects.None);
                    Main.EntitySpriteDraw(prism3, Projectile.Center - Main.screenPosition, prismSrc, Color.White, Projectile.ai[0] / 15f, prismSrc.Size() * 0.5f, 0.5f, SpriteEffects.None);

                    for (int i = 0; i < 3; i++)
                    {
                        float progress = (Projectile.ai[0] - COLLISION_TIME) / WAIT_TIME;
                        progress -= i * 0.3f;
                        progress = Math.Clamp(progress, 0f, 1f);

                        float ease = MathF.Sin(progress * MathF.PI * 0.5f);
                        float scale = ease * 2f;
                        float opacity = 1f - ease;

                        Main.EntitySpriteDraw(ring, Projectile.Center - Main.screenPosition, prismSrc, new Color(1f, 1f, 1f, opacity), 0f, prismSrc.Size() * 0.5f, scale, SpriteEffects.None);
                    }

                    Texture2D glareTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/VFXSprites/HollowPurpleGlare{glareIndex + 1}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                    if (Projectile.ai[0] % 3 == 0)
                        glareIndex = Main.rand.Next(0, 3);

                    Rectangle glareSrc = new Rectangle(0, 0, glareTexture.Width, glareTexture.Height);
                    Main.EntitySpriteDraw(glareTexture, Projectile.Center - Main.screenPosition, glareSrc, Color.White, Projectile.rotation, glareSrc.Size() * 0.5f, 1.5f, SpriteEffects.None);


                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin();
                }
            }

            int flashDuration = 10;
            if (Projectile.ai[0] >= COLLISION_TIME - (flashDuration / 2))
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

                float progress = (Projectile.ai[0] - (COLLISION_TIME - flashDuration / 2f)) / flashDuration;
                progress = Math.Clamp(progress, 0f, 1f);

                if (Projectile.ai[0] >= COLLISION_TIME + (flashDuration / 2) && Projectile.ai[0] < COLLISION_TIME + WAIT_TIME)
                    progress = 0.1f;

                float opacity = MathF.Sin(progress * MathF.PI);
                opacity = Math.Clamp(opacity, 0f, 1f);

                Rectangle flashSrc = new Rectangle(0, 0, flash.Width, flash.Height);
                Main.EntitySpriteDraw(flash, Projectile.Center - Main.screenPosition, flashSrc, new Color(textColor.R, textColor.G, textColor.B, opacity), Projectile.rotation, flashSrc.Size() * 0.5f, 2.5f, SpriteEffects.None);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
            }

            if (Projectile.ai[0] >= COLLISION_TIME + WAIT_TIME)
            {
                int frameHeight = texture.Height / FRAME_COUNT;
                int frameY = Projectile.frame * frameHeight;
                Rectangle src = new Rectangle(0, frameY, texture.Width, frameHeight);
                Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.rotation, src.Size() * 0.5f, 2f, SpriteEffects.None);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 20; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7));

                LineParticle particle = new LineParticle(target.Center, Projectile.velocity + variation, false, 30, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }
    }
}