using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
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

        public static readonly int FRAME_COUNT = 1;
        public static readonly int TICKS_PER_FRAME = 1;
        public static Texture2D texture;

        public override string InternalName => "GarudaKick";

        public bool animating;
        public float animScale;

        // public override float Cost => 40f;
        // public override int Damage => 30;
        // public override int MasteryDamageMultiplier => 50;
        // public override float Speed => 25f;
        // public override float LifeTime => 300f;

        public GarudaKick()
        {
            Technique.baseDamage = 10;
            Technique.damagePerBoss = 6;
            Technique.cost = 10;
            Technique.speed = 25f;
        }
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }


        //public override bool UseCondition(SorceryFightPlayer sf)
        //{
        //    return !sf.summonGaruda;
        //}


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
            Projectile.rotation += MathHelper.ToRadians(10f);
            Projectile.ai[0] += 1;
            float beginAnimTime = 30f;
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] > lifetime + beginAnimTime)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] < beginAnimTime)
            {
                if (!animating)
                {
                    Projectile.Center += new Vector2(0, 30);
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);
                }

                Dust dust = Dust.NewDustDirect(
                    Projectile.Center - new Vector2(4, 4),
                    8, 8,
                    DustID.Smoke,
                    -Projectile.velocity.X * 0.3f,
                    -Projectile.velocity.Y * 0.3f,
                    150,
                    new Color(180, 180, 180),
                    Main.rand.NextFloat(0.8f, 1.4f)
                );
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
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

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 6; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, new Color(255, 0, 0), false, 0.9f, 1f, 30);
                ParticleController.SpawnParticle(particle);
            }
        }


        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * speed;
                var entitySource = player.GetSource_FromThis();

                if (!sf.summonGaruda)
                {
                    return Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), CalculateTrueDamage(sf), 0, player.whoAmI);
                }
                else
                {
                    return Projectile.NewProjectile(entitySource, player.Center, dir, ModContent.ProjectileType<MassKick>(), CalculateTrueDamage(sf), 0, player.whoAmI);
                }
            }
            return -1;
        }

    }
}