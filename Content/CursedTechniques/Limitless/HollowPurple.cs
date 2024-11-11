using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class HollowPurple : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;

        public override string Name { get; set; } = "Hollow Technique: Purple";
        public override string Description
        { 
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.HollowPurple.Description").Value;
            }
        }
        public override string LockedDescription
        {
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.HollowPurple.LockedDescription").Value;
            }
        }
        public override float Cost { get; set; } = 500f;
        public override float MasteryNeeded { get; set; } = 0f;
        public override Color textColor { get; set; } = new Color(235, 117, 233);

        public override int Damage { get; set ; } = 5000;
        public override float Speed { get; set; } = 35f;
        public override float LifeTime { get; set; } = 500f;
        public override bool Unlocked
        {
            get
            {
                return NPC.downedMoonlord;
            }
        }

        public static Texture2D texture;

        public bool animating;
        public float animScale;
        public Rectangle hitbox;
        public Vector2 blueOffset;
        public Vector2 redOffset;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<HollowPurple>();
        } 

        public override float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 50;
            animating = false;
            animScale = 1f;
            hitbox = Projectile.Hitbox;

            blueOffset = new Vector2(-60f, -20f);
            redOffset = new Vector2(60f, -20f);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1;
            Player player = Main.player[Projectile.owner];

            float beginAnimTime = 360f;

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
                }

                animScale = 0f;
                Projectile.damage = 0;
                Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
                Projectile.Center = player.Center + new Vector2(0f, -30f);                


                Vector2 bluePosition = player.Center + blueOffset;
                Vector2 redPosition = player.Center + redOffset;

                if (Projectile.ai[0] == 1)
                {
                    if (Main.myPlayer == Projectile.owner)
                    { 
                        int index = Projectile.NewProjectile(Projectile.GetSource_FromThis(), bluePosition, Vector2.Zero, ModContent.ProjectileType<AmplificationBlue>(), 0, 0f, Projectile.owner, default, 1);
                        if (index >= 0) 
                            Projectile.ai[1] = index;
                    }
                }


                if (Projectile.ai[0] == 30)
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

                if (Projectile.ai[0] >= 1 && blue.type == ModContent.ProjectileType<AmplificationBlue>())
                {
                    blue.Center = bluePosition;
                    
                    Vector2 particleOffsetPosition = bluePosition + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                    Vector2 particleVelocity = particleOffsetPosition.DirectionTo(player.Center + new Vector2(0f, -20f)) * 2;
                    LineParticle particle = new LineParticle(particleOffsetPosition, particleVelocity, false, 30, 0.5f, new Color(108, 158, 240));
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                if (Projectile.ai[0] >= 30 && red.type == ModContent.ProjectileType<ReversalRed>())
                {
                    red.Center = redPosition;

                    Vector2 particleOffsetPosition = redPosition + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                    Vector2 particleVelocity = particleOffsetPosition.DirectionTo(player.Center + new Vector2(0f, -20f)) * 2;
                    LineParticle particle = new LineParticle(particleOffsetPosition, particleVelocity, false, 30, 0.5f, new Color(224, 74, 74));
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                if (Projectile.ai[0] == 95)
                    SoundEngine.PlaySound(SorceryFightSounds.CommonWoosh, Projectile.Center);

                if (Projectile.ai[0] > 120)
                {
                    this.blueOffset.X += 2f;
                    this.redOffset.X -= 2f;
                    
                    if (this.blueOffset.X >= this.redOffset.X)
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
                animScale = 1f;
                Projectile.damage = Damage;
                Projectile.Hitbox = hitbox;
                Projectile.timeLeft = (int)LifeTime;
                Main.projectile[(int)Projectile.ai[1]].Kill();
                Main.projectile[(int)Projectile.ai[2]].Kill();
                Projectile.Center = player.Center + new Vector2(0f, -40f);
                Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleSnap, Projectile.Center);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/HollowPurple").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool Shoot(Terraria.DataStructures.IEntitySource spawnSource, Vector2 position, Vector2 velocity, Player player)
        {
            if (base.Shoot(spawnSource, position, velocity, player) && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(spawnSource, position, velocity, ModContent.ProjectileType<HollowPurple>(), Damage, 0f, player.whoAmI);
            }
            return true;
        }
    }
}