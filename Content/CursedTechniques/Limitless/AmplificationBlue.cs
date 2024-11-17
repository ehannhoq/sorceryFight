using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class AmplificationBlue : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8; 
        public static readonly int TICKS_PER_FRAME = 5;

        public override string Name { get; set; } = "Cursed Technique Amplification: Blue";
        public override string Description
        { 
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.AmplificationBlue.Description").Value;
            }
        }
        public override string LockedDescription
        {
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.AmplificationBlue.LockedDescription").Value;
            }
        }

        public override float Cost { get; set; } = 50f;
        public override float MasteryNeeded { get; set; } = 0f;
        public override Color textColor { get; set; } = new Color(108, 158, 240);
        public override bool Unlocked
        {
            get
            {
                return NPC.downedBoss3;
            }
        }

        public override int Damage { get; set ; } = 150;
        public override float Speed { get; set; } = 15f;
        public override float LifeTime { get; set; } = 300f;
        public virtual float AttractionRadius { get; set; } = 100f;
        public virtual float AttractionStrength { get; set; } = 12f;

        public static Texture2D texture;

        public bool animating;
        public float animScale;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<AmplificationBlue>();
        } 

        public override float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = true;
            animating = false;
            animScale = 0.75f;
        }
        public override void AI()
        {
            Projectile.ai[0] += 1;
            float beginAnimTime = 30f;
            bool spawnedFromPurple = Projectile.ai[1] == 1;
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

            foreach (Projectile proj in Main.projectile)
            {
                if (!proj.hostile && proj != Main.projectile[Projectile.whoAmI])
                {
                    float distance = Vector2.Distance(proj.Center, Projectile.Center);
                    
                    if (distance <= AttractionRadius)
                    {
                        Vector2 direction = proj.Center.DirectionTo(Projectile.Center);
                        Vector2 newVelocity = Vector2.Lerp(proj.velocity, direction * AttractionStrength, 0.1f);

                        proj.velocity = newVelocity;
                    }
                }
            }

            foreach (NPC npc in Main.npc)
            {
                if (!npc.friendly && npc.type != NPCID.TargetDummy)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance <= AttractionRadius)
                    {
                        Vector2 direction = npc.Center.DirectionTo(Projectile.Center);
                        Vector2 newVelocity = Vector2.Lerp(npc.velocity, direction * AttractionStrength, 0.1f);

                        npc.velocity = newVelocity;
                    }
                }
            }

            if (Projectile.ai[0] < beginAnimTime)
            {
                if (!animating)
                {
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);

                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                }

                float goalScale = 0.75f;

                if (spawnedFromPurple) // Hook for if the projectile was spawned by a Hollow Purple
                    goalScale = 1.25f;

                if (animScale < goalScale)
                    animScale = Projectile.ai[0] / beginAnimTime;
                else
                    animScale = goalScale;

                
                Vector2 particleOffset = Projectile.Center + new Vector2(Main.rand.NextFloat(-40f, 40f), Main.rand.NextFloat(-40f, 40f));
                Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 10, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);

                return;
            }

            if (animating)
            {
                Projectile.tileCollide = true;
                animating = false;

                if (!spawnedFromPurple)
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/AmplificationBlue").Value;


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

            for (int i = 0; i < 6; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LineParticle particle = new LineParticle(target.Center, Projectile.velocity + variation, false, 30, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }

        public override bool Shoot(Terraria.DataStructures.IEntitySource spawnSource, Vector2 position, Vector2 velocity, Player player)
        {
            if (base.Shoot(spawnSource, position, velocity, player) && Main.myPlayer == player.whoAmI)
                Projectile.NewProjectile(spawnSource, position, velocity, ModContent.ProjectileType<AmplificationBlue>(), Damage, 0f, player.whoAmI);
            return true;
        }
    }
}