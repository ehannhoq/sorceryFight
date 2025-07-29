
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using CalamityMod.NPCs.Providence;
using Terraria.Graphics.Effects;
using System.IO;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class MaximumOutputRed : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 9;
        public static readonly int TICKS_PER_FRAME = 3;
        public static Texture2D texture;


        public bool inAnimation;
        public ref float scale => ref Projectile.ai[2];

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.MaximumOutputRed.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.MaximumOutputRed.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.MaximumOutputRed.LockedDescription");
        public override float Cost { get; } = 750f;
        public override Color textColor { get; } = new Color(224, 74, 74);
        public override int Damage => 4500;
        public override int MasteryDamageMultiplier => 310;
        public override float Speed { get; } = 23f;
        public override float LifeTime { get; } = 180f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<Providence>());
        }
        public override bool DisplayNameInGame { get; } = true;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<MaximumOutputRed>();
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
            Projectile.timeLeft = (int)LifeTime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;

            inAnimation = false;
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

            float beginPhaseTime = 60f;


            if (Projectile.ai[0] < beginPhaseTime)
            {
                if (!Main.dedServ && Projectile.owner == Main.myPlayer)
                {
                    float percent = Projectile.ai[0] / beginPhaseTime;
                    float pixelRadius = 200f * (1f - percent);
                    float radius = pixelRadius / Main.screenWidth;
                    
                    if (!Filters.Scene["SF:MaximumRed"].IsActive())
                    {
                        Filters.Scene.Activate("SF:MaximumRed").GetShader().UseColor(textColor).UseOpacity(1f);
                    }
                    else
                    {
                        Filters.Scene["SF:MaximumRed"].GetShader().UseTargetPosition(Projectile.Center).UseProgress(radius);

                        if (percent >= 0.95f)
                            Filters.Scene["SF:MaximumRed"].GetShader().UseOpacity(0f);
                    }
                }

                if (!inAnimation)
                {
                    inAnimation = true;
                    Projectile.damage = 0;
                    scale = 0;
                    SoundEngine.PlaySound(SorceryFightSounds.ReversalRedChargeUp, Projectile.Center);
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                }


                if (!spawnedFromPurple)
                    Projectile.Center = player.Center;
                else
                    scale = Projectile.ai[0] / beginPhaseTime;

                Projectile.netUpdate = true;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 particleOffset = Projectile.Center + new Vector2(Main.rand.NextFloat(-80f, 80f), Main.rand.NextFloat(-80f, 80f));
                    Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                    LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 10, 1, textColor);
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                return;
            }
            else
            {
                if (inAnimation)
                {
                    inAnimation = false;
                    scale = 1;

                    if (!spawnedFromPurple)
                    {
                        SoundEngine.PlaySound(SorceryFightSounds.CommonFire, Projectile.Center);
                        Projectile.damage = (int)CalculateTrueDamage(player.GetModPlayer<SorceryFightPlayer>());
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (!spawnedFromPurple)
                        {
                            Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                            player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                        }

                        if (Filters.Scene["SF:MaximumRed"].IsActive())
                        {
                            Filters.Scene["SF:MaximumRed"].Deactivate();
                        }
                    }

                    Projectile.netUpdate = true;
                }
            }


        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(inAnimation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            inAnimation = reader.ReadBoolean();
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
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/MaximumOutputRed").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}