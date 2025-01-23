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
using sorceryFight.SFPlayer;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class AmplificationBlue : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 5;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.AmplificationBlue.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.AmplificationBlue.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.AmplificationBlue.LockedDescription");
        public override float Cost => 50f;
        public override Color textColor => new Color(108, 158, 240);
        public override bool DisplayNameInGame => true;

        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 55;

        public override float Speed => 20f;
        public override float LifeTime => 300f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }




        public virtual float AttractionRadius { get; } = 100f;
        public virtual float AttractionStrength { get; } = 12f;

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


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 65;
            Projectile.height = 65;
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
                LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 10, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);

                return;
            }

            if (animating)
            {
                Projectile.tileCollide = true;
                animating = false;
            }

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile && proj != Main.projectile[Projectile.whoAmI] && proj.MoveableByBlue())
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
                if (!npc.friendly && npc.type != NPCID.TargetDummy && !npc.IsDomain() && npc.MoveableByBlue())
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

    }
}