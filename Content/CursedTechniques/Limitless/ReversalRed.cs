
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class ReversalRed : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 7; 
        public static readonly int TICKS_PER_FRAME = 5;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.ReversalRed.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.ReversalRed.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.ReversalRed.LockedDescription");
        public override float Cost { get; } = 250f;
        public override Color textColor { get; } = new Color(224, 74, 74);

       
        public override int Damage => 5000;
        public override int MasteryDamageMultiplier => 333;
        public override float Speed { get; } = 30f;
        public override float LifeTime { get; } = 300f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.unlockedRCT;
        }

        public static Texture2D texture;
        public override bool DisplayNameInGame { get; } = true;

        public bool animating;
        public float animScale;
        public Rectangle hitbox;
        
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<ReversalRed>();
        } 

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.tileCollide = false;
            animating = false;
            animScale = 0f;
            hitbox = Projectile.Hitbox;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Projectile.ai[0]++;
            bool spawnedFromPurple = Projectile.ai[1] == 1;
            Player player = Main.player[Projectile.owner];

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }


            float beginAnim = 60f;

            if (Projectile.ai[0] < beginAnim)
            {
                if (!animating)
                {
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.ReversalRedChargeUp, Projectile.Center);
                    Projectile.damage = 0;
                    Projectile.tileCollide = false;
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                }

                if (!spawnedFromPurple)
                    Projectile.Center = player.Center;
                else
                {
                    float goalScale = 2f;

                    if (animScale < goalScale)
                    {
                        animScale = (Projectile.ai[0] / beginAnim) * goalScale;
                    }
                    else
                        animScale = goalScale;
                }

                for (int i = 0; i < 2; i++)
                {
                    Vector2 particleOffset = Projectile.Center + new Vector2(Main.rand.NextFloat(-80f, 80f), Main.rand.NextFloat(-80f, 80f));
                    Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                    LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 10, 1, textColor);
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                return;
            }

            if (animating)
            {
                animating = false;
                Projectile.velocity = Vector2.Zero;
                animScale = 2f;

                if (!spawnedFromPurple)
                {
                    SoundEngine.PlaySound(SorceryFightSounds.CommonFire, Projectile.Center);
                    Projectile.damage = (int)CalculateTrueDamage(player.GetModPlayer<SorceryFightPlayer>());
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;

                    if (Main.myPlayer == Projectile.owner)
                        Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 10; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7));

                LineParticle particle = new LineParticle(target.Center, Projectile.velocity + variation, false, 30, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/ReversalRed").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);
            return false;
        }

        private void SpecialKill()
        {
            for (int i = 0; i < 40; i++)
            {
                Vector2 particleOffset = Projectile.Center + new Vector2(Main.rand.NextFloat(-120f, 120f), Main.rand.NextFloat(-120f, 120f));
                Vector2 particleVelocity = particleOffset.DirectionFrom(Projectile.Center);
                LineParticle particle = new LineParticle(Projectile.Center, particleVelocity * 3, false, 20, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }

            Projectile.Kill();
        }
    }
}