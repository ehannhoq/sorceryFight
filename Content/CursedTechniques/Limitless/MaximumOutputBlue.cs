using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Particles;

using sorceryFight.Utilities.EaseFunctions;


namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class MaximumOutputBlue : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8; 
        public static readonly int TICKS_PER_FRAME = 5;

        public override string InternalName => "MaximumOutputBlue";

        public virtual float AttractionRadius { get; set; } = 130f;
        public virtual float AttractionStrength { get; set; } = 15f;

        public static Texture2D texture;

        public bool animating;
        public float animScale;
        public bool justSpawned;
        public bool followCursor = true;


        public MaximumOutputBlue()
        {
            Technique.baseDamage = 10;
            Technique.damagePerBoss = 3;
            Technique.cost = 80;
            Technique.speed = 20;
            Technique.lifetime = 300;
        }


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 75;
            Projectile.height = 75;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            animating = false;
            animScale = 0f;
            justSpawned = true;
        }


        public override void AI()
        {   
            if (justSpawned)
            {
                Projectile.Center += new Vector2(0, -20f);
                for (int i = 0; i < Main.projectile.Length; i ++)
                {
                    if (i == Projectile.whoAmI)
                        continue;

                    Projectile proj = Main.projectile[i];

                    if (proj.type == ModContent.ProjectileType<MaximumOutputBlue>() && proj.owner == Projectile.owner)
                    {
                        proj.Kill();
                    }
                }

                justSpawned = false;
            }

            Projectile.ai[0] += 1;
            float beginAnimTime = 30;
            Player player = Main.player[Projectile.owner];

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.ai[0] > lifetime - beginAnimTime)
            {
                float timeSince = Projectile.ai[0] - (lifetime - beginAnimTime);
                float progress = timeSince / beginAnimTime;
                animScale = 1.25f * (1 - EaseFunctions.EaseInOut(progress));
                if (Projectile.ai[0] > lifetime - 2)
                    SpecialKill();
            }
    

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.hostile && proj != Main.projectile[Projectile.whoAmI] && proj.MoveableByBlue())
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

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.friendly && npc.type != NPCID.TargetDummy && npc.MoveableByBlue())
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

            if (followCursor)
            {
                if (Main.mouseRight && Main.mouseRightRelease)
                    followCursor = false;
            }
            else
                    Projectile.velocity *= 0.9f;
                    

            if (Main.myPlayer == Projectile.owner && followCursor)
            {
                Vector2 projDirection = Projectile.Center.DirectionTo(Main.MouseWorld);
                Vector2 projVelocity = Vector2.Lerp(Projectile.velocity, projDirection * 30f, 0.1f);
                Projectile.velocity = projVelocity;
            }


            if (Projectile.ai[0] < beginAnimTime)
            {
                if (!animating)
                {
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);
                }

                float goalScale = 1.25f;
                if (animScale < goalScale)
                    animScale = (Projectile.ai[0] / beginAnimTime) * goalScale;
                else
                    animScale = goalScale;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 particleOffset = Projectile.Center + new Vector2(Main.rand.NextFloat(-80f, 80f), Main.rand.NextFloat(-80f, 80f));
                    Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                    LinearParticle particle = new LinearParticle(particleOffset, particleVelocity * 3, new Color(108, 218, 240), false, 0.9f, 1f);
                    ParticleController.SpawnParticle(particle);
                }

                return;
            }

            if (animating)
            {
                Projectile.tileCollide = true;
                animating = false;
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 10; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7));

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, new Color(108, 218, 240), false, 0.9f, 1, 30);
                ParticleController.SpawnParticle(particle);
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/MaximumOutputBlue").Value;


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
                LinearParticle particle = new LinearParticle(Projectile.Center, particleVelocity * 3, new Color(108, 218, 240), false, 0.9f, 1, 20);
                ParticleController.SpawnParticle(particle);
            }
            Projectile.Kill();
        }
    }
}