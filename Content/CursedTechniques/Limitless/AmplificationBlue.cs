using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class AmplificationBlue : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 5;

        public override string InternalName => "AmplificationBlue";

        public float attractionRadius = 100f;
        public float attractionStrength = 12f;

        public static Texture2D texture;

        public bool animating;
        public float animScale;


        public AmplificationBlue()
        {
            Technique.baseDamage = 30;
            Technique.damagePerBoss = 10;
            Technique.cost = 20;
            Technique.speed = 15;
        }


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;

            if (!Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/AmplificationBlue", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.tileCollide = true;
            animating = false;
            animScale = 1.25f;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1;
            float beginAnimTime = 30f;
            bool spawnedFromPurple = Projectile.ai[1] == 1;
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] > lifetime + beginAnimTime)
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

                float goalScale = 1.25f;

                if (spawnedFromPurple)
                {
                    Projectile.tileCollide = false;
                    goalScale = 3f;
                }


                if (animScale < goalScale)
                    animScale = Projectile.ai[0] / beginAnimTime;
                else
                    animScale = goalScale;


                Vector2 particleOffset = Projectile.Center + new Vector2(Main.rand.NextFloat(-40f, 40f), Main.rand.NextFloat(-40f, 40f));
                Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                LinearParticle particle = new LinearParticle(particleOffset, particleVelocity * 3, new Color(108, 218, 240), false, 0.9f, 1f, 10);
                ParticleController.SpawnParticle(particle);

                return;
            }

            if (animating)
            {
                if (!spawnedFromPurple)
                    Projectile.tileCollide = true;
                animating = false;
            }

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile && proj != Main.projectile[Projectile.whoAmI] && proj.MoveableByBlue())
                {
                    float distance = Vector2.Distance(proj.Center, Projectile.Center);

                    if (distance <= attractionRadius)
                    {
                        Vector2 direction = proj.Center.DirectionTo(Projectile.Center);
                        Vector2 newVelocity = Vector2.Lerp(proj.velocity, direction * attractionStrength, 0.1f);

                        proj.velocity = newVelocity;
                    }
                }
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.friendly && npc.type != NPCID.TargetDummy && npc.MoveableByBlue())
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance <= attractionRadius)
                    {
                        Vector2 direction = npc.Center.DirectionTo(Projectile.Center);
                        Vector2 newVelocity = Vector2.Lerp(npc.velocity, direction * attractionStrength, 0.1f);

                        npc.velocity = newVelocity;
                    }
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

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

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, new Color(108, 218, 240), false, 0.9f, 1, 30);
                ParticleController.SpawnParticle(particle);
            }
        }

    }
}