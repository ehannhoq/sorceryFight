using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using CalamityMod.NPCs.DevourerofGods;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class HollowPurple200Percent : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.HollowPurple200Percent.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.HollowPurple200Percent.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.HollowPurple200Percent.LockedDescription");
        public override float Cost { get; } = 1100f;
        public override Color textColor { get; } = new Color(235, 117, 233);
        public override bool DisplayNameInGame { get; } = false;

        
        public override int Damage => 90000;
        public override int MasteryDamageMultiplier => 222;
        public override float Speed { get; } = 50f;
        public override float LifeTime { get; } = 500f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
        }

        public static Texture2D texture;

        public bool animating;
        public float animScale;
        public Rectangle hitbox;
        public Vector2 blueOffset;
        public Vector2 redOffset;
        public int incantationsIndex;
        public List<string> incantations;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<HollowPurple200Percent>();
        } 

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 250;
            Projectile.height = 250;
            animating = false;
            animScale = 2.5f;
            hitbox = Projectile.Hitbox;

            blueOffset = new Vector2(-60f, -20f);
            redOffset = new Vector2(60f, -20f);
            incantationsIndex = 0;
            incantations = new List<string>()
            {
                "Nine Ropes.",
                "Polarized Light.",
                "Crow and Declaration.",
                "Between Front and Back.",
            };
        }

        public override void AI()
        {
            Projectile.ai[0] += 1;
            Player player = Main.player[Projectile.owner];

            float beginAnimTime = 600f;

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
                    animating = true;
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                }

                animScale = 0f;
                Projectile.damage = 0;
                Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
                Projectile.Center = player.Center + new Vector2(0f, -30f);

                if (Projectile.ai[0] % 90 == 1)
                {
                    int index = CombatText.NewText(player.getRect(), textColor, incantations[incantationsIndex]);
                    Main.combatText[index].lifeTime = 60;

                    if (incantationsIndex < incantations.Count)
                        incantationsIndex++;
                }


                Vector2 bluePosition = player.Center + blueOffset;
                Vector2 redPosition = player.Center + redOffset;

                if (Projectile.ai[0] == 90)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), bluePosition, Vector2.Zero, ModContent.ProjectileType<AmplificationBlue>(), 0, 0f, Projectile.owner, default, 1);
                        if (index >= 0)
                            Projectile.ai[1] = index;
                    }
                }


                if (Projectile.ai[0] == 180)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), redPosition, Vector2.Zero, ModContent.ProjectileType<ReversalRed>(), 0, 0f, Projectile.owner, default, 1);
                        if (index >= 0)
                            Projectile.ai[2] = index;
                    }
                }

                Projectile blue = Main.projectile[(int)Projectile.ai[1]];
                Projectile red = Main.projectile[(int)Projectile.ai[2]];

                if (Projectile.ai[0] >= 90 && blue.type == ModContent.ProjectileType<AmplificationBlue>())
                { 
                    blue.Center = bluePosition;

                    Vector2 particleOffsetPosition = bluePosition + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                    Vector2 particleVelocity = particleOffsetPosition.DirectionTo(player.Center + new Vector2(0f, -20f)) * 2;
                    LineParticle particle = new LineParticle(particleOffsetPosition, particleVelocity, false, 30, 0.5f, new Color(108, 158, 240));
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                if (Projectile.ai[0] >= 180 && red.type == ModContent.ProjectileType<ReversalRed>())
                {
                    red.Center = redPosition;

                    Vector2 particleOffsetPosition = redPosition + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                    Vector2 particleVelocity = particleOffsetPosition.DirectionTo(player.Center + new Vector2(0f, -20f)) * 2;
                    LineParticle particle = new LineParticle(particleOffsetPosition, particleVelocity, false, 30, 0.5f, new Color(224, 74, 74));
                    GeneralParticleHandler.SpawnParticle(particle);
                }
                if (Projectile.ai[0] == 280)
                    SoundEngine.PlaySound(SorceryFightSounds.CommonWoosh, Projectile.Center);

                if (Projectile.ai[0] >= 320)
                {
                    blueOffset.X += 2f;
                    redOffset.X -= 2f;

                    if (blueOffset.X >= redOffset.X)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            Vector2 offsetParticlePosition = Projectile.Center + new Vector2(Main.rand.NextFloat(-300, 300), Main.rand.NextFloat(-300, 300));
                            Vector2 offsetParticleVelocity = Projectile.Center.DirectionTo(offsetParticlePosition) * 10;

                            AltSparkParticle particle = new AltSparkParticle(Projectile.Center, offsetParticleVelocity, false, 45, 1.5f, Color.White);
                            GeneralParticleHandler.SpawnParticle(particle);
                        }

                        Projectile.ai[0] = beginAnimTime;
                    }
                }

                return;
            }

            if (animating)
            {
                animating = false;
                animScale = 2.5f;
                Projectile.damage = (int)CalculateTrueDamage(player.GetModPlayer<SorceryFightPlayer>());
                Projectile.Hitbox = hitbox;
                Projectile.timeLeft = (int)LifeTime;
                Main.projectile[(int)Projectile.ai[1]].Kill();
                Main.projectile[(int)Projectile.ai[2]].Kill();
                Projectile.Center = player.Center + new Vector2(0f, -40f);
                SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleSnap, Projectile.Center);
                player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                int index = CombatText.NewText(player.getRect(), textColor, "Hollow Technique: 200% Hollow Purple.");
                Main.combatText[index].lifeTime = 180;

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                    player.AddBuff(ModContent.BuffType<BurntTechnique>(), SorceryFight.BuffSecondsToTicks(5));
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 40; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7));

                LineParticle particle = new LineParticle(target.Center, Projectile.velocity + variation, false, 30, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/HollowPurple200Percent").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}