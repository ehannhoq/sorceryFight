using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class GarudaKick : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 5;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.GarudaKick.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.GarudaKick.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.GarudaKick.LockedDescription");
        public override float Cost => 40f;

        public override float BloodCost => 20f;

        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;

        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 25f;
        public override float LifeTime => 300f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }


        public static Texture2D texture;

        public bool animating;
        public float animScale;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<GarudaKick>();
        }

        public override bool UseCondition(SorceryFightPlayer sf)
        {
            return !sf.summonGaruda;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 65;
            Projectile.height = 65;
            Projectile.tileCollide = true;
            animating = false;
            Projectile.penetrate = -1;
            animScale = 1.25f;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0] += 1;
            float beginAnimTime = 30f;
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] > LifeTime + beginAnimTime)
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

            if (Projectile.ai[0] < beginAnimTime)
            {
                if (!animating)
                {
                    Projectile.Center += new Vector2(0, -30);
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);
                }

                //Code that was for expanding the spread of the particles based on height of shooting, but rotating the projectile itself looked better 
                //float verticalness = 1f - Math.Abs(Projectile.velocity.SafeNormalize(Vector2.Zero).X);
                //float spreadWidth = MathHelper.Lerp(8f, 60f, verticalness);
                //Vector2 behindOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(30f, 120f);
                //Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-spreadWidth, spreadWidth);
                //Vector2 particleOffset = Projectile.Center + behindOffset + perpendicular;
                //Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                //LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 20, 1f, textColor);
                //GeneralParticleHandler.SpawnParticle(particle);

                Vector2 behindOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10f, 40f);
                Vector2 particleOffset = Projectile.Center + behindOffset;
                Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 20, 1f, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
                return;
            }

            if (animating)
            {
                Projectile.tileCollide = true;
                animating = false;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/StarRage/GarudaKick").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.penetrate = 0;

            target.AddBuff(BuffID.Poisoned, 300);

            for (int i = 0; i < 6; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LineParticle particle = new LineParticle(target.Center, Projectile.velocity + variation, false, 30, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }

    }
}