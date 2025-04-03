using System;
using System.Collections.Generic;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class DivineFlame : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 9;
        public static readonly int TICKS_PER_FRAME = 2;
        static List<Texture2D> textures;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.DivineFlame.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineFlame.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineFlame.LockedDescription");
        public override float Cost => 525f;
        public override Color textColor => new Color(242, 144, 82);
        public override bool DisplayNameInGame => false;
        public override int Damage => 21000;
        public override int MasteryDamageMultiplier => 444;
        public override float Speed => 30f;
        public override float LifeTime => 300f;

        ref float animTimer => ref Projectile.ai[0];
        Rectangle hitbox;
        int texturePhase; // 0 -> Fire strands. 1 -> Fire arrow

        bool animating;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<DivineFlame>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<Providence>()) || sf.Player.HasBuff(ModContent.BuffType<KingOfCursesBuff>());
        }

        public override float CalculateTrueDamage(SorceryFightPlayer sf)
        {
            return base.CalculateTrueDamage(sf) * (1 + (0.01f * sf.sukunasFingerConsumed));
        }

        public override float CalculateTrueCost(SorceryFightPlayer sf)
        {
            return base.CalculateTrueCost(sf) * (1 - (0.01f * sf.sukunasFingerConsumed));
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            textures = new List<Texture2D>()
            {
                ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/DivineFlameStrands", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/DivineFlame", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            };
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 101;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            animating = false;
            hitbox = Projectile.Hitbox;
            texturePhase = 0;
        }
        public override void AI()
        {
            animTimer++;
            float animTime = 180f;
            Player player = Main.player[Projectile.owner];

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = FRAME_COUNT - 1;
                }
            }

            if (animTimer < animTime)
            {
                if (!animating)
                {
                    animating = true;
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                    Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
                    Projectile.damage = 0;
                    texturePhase = 0;

                }

                if (animTimer == 1)
                    SoundEngine.PlaySound(SorceryFightSounds.DivineFlameChargeUp with { Volume = 2f }, player.Center);

                Projectile.Center = player.Center;
                Projectile.timeLeft = 30;

                if (animTimer < 30)
                {
                    Vector2 pos = Projectile.Center;
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                    GlowSparkParticle particle = new GlowSparkParticle(pos, velocity, false, 60, 0.01f, textColor, new Vector2(1, 1));
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                if (animTimer == 30)
                {
                    texturePhase = 1;
                    int index = CombatText.NewText(player.getRect(), textColor, "Divine Flame");
                    Main.combatText[index].lifeTime = 60;
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 pos = Projectile.Center;
                        Vector2 velocity = new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-50f, 50f));
                        GlowSparkParticle particle = new GlowSparkParticle(pos, velocity, false, 60, 0.1f, textColor, new Vector2(1, 1));
                        GeneralParticleHandler.SpawnParticle(particle);
                    }
                }

                if (animTimer == 120)
                {
                    int index = CombatText.NewText(player.getRect(), textColor, "Open.");
                    Main.combatText[index].lifeTime = 180;
                }

                if (animTimer > 30)
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation();


                    Vector2 pos = Projectile.Center;
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                    GlowSparkParticle particle = new GlowSparkParticle(pos, velocity, false, 60, 0.02f, textColor, new Vector2(1, 1));
                    GeneralParticleHandler.SpawnParticle(particle);
                }
                return;
            }

            if (animating)
            {
                animating = false;
                Projectile.damage = (int)CalculateTrueDamage(player.GetModPlayer<SorceryFightPlayer>());
                Projectile.width = 227;
                Projectile.height = 49;
                Projectile.Hitbox = hitbox;
                Projectile.timeLeft = (int)LifeTime;
                Projectile.Center = player.Center;

                SoundEngine.PlaySound(SorceryFightSounds.DivineFlameShoot, Projectile.Center);
                player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                }
            }

            float velocityRotation = Projectile.velocity.ToRotation();
            Projectile.direction = (Math.Cos(velocityRotation) > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 pos2 = Projectile.Center;
            Vector2 velocity2 = new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-50f, 50f));
            GlowSparkParticle particle2 = new GlowSparkParticle(pos2, velocity2, false, 60, 0.05f, textColor, new Vector2(1, 1));
            GeneralParticleHandler.SpawnParticle(particle2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (texturePhase == 0)
            {
                int frameHeight = textures[texturePhase].Height / FRAME_COUNT;
                int frameY = Projectile.frame * frameHeight;

                Vector2 origin = new Vector2(textures[texturePhase].Width / 2, frameHeight / 2);
                Rectangle sourceRectangle = new Rectangle(0, frameY, textures[texturePhase].Width, frameHeight);
                Main.spriteBatch.Draw(textures[texturePhase], Main.LocalPlayer.Center - Main.screenPosition + new Vector2(0f, -30f), sourceRectangle, Color.White, Projectile.rotation + (MathHelper.Pi / 6), origin, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                Vector2 origin = new Vector2(textures[texturePhase].Width / 2, textures[texturePhase].Height / 2);
                Main.spriteBatch.Draw(textures[texturePhase], Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);

            }

            return false;
        }
    }
}
