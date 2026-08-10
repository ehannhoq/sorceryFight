using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;


namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class SuperNova : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 3;
        public static readonly int TICKS_PER_FRAME = 5;
        public static Texture2D texture;

        public override string InternalName => "SuperNova";


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }


        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 mousePos = Main.MouseWorld;
                var entitySource = player.GetSource_FromThis();
                int index = Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, GetProjectileType(), CalculateTrueDamage(sf), 0f, player.whoAmI);
                Main.projectile[index].ai[0] = Main.MouseWorld.X;
                Main.projectile[index].ai[1] = Main.MouseWorld.Y;
                return index;
            }
            return -1;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }


        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            Vector2 targetPos = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            if (Vector2.Distance(Projectile.Center, targetPos) > speed)
            {
                Projectile.velocity = Projectile.DirectionTo(targetPos) * speed;
            }
            else
            {
                Projectile.Center = targetPos;
                Projectile.velocity = Vector2.Zero;
                Projectile.penetrate = 1;
                Projectile.tileCollide = true;
            }
        }


        public override void OnKill(int timeLeft)
        {
            //only create shotgun blast when it expires naturaully (2 seconds)
            if (timeLeft == 0)
            {
                for (int i = 0; i < 16; i++)
                {
                    float angle = MathHelper.TwoPi / 16 * i;
                    Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 10f;
                    Projectile.NewProjectile(Projectile.GetSource_Death(), Projectile.Center, velocity, ModContent.ProjectileType<SuperNovaShard>(), 500, Projectile.knockBack, Projectile.owner);
                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/SuperNova").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 1.25f, SpriteEffects.None, 0f);

            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            int paintingCount = Main.player[Projectile.owner].SorceryFight().deathPaintings.Count(p => p);
            target.AddBuff(ModContent.BuffType<BloodPoison>(), paintingCount * 60);

            for (int i = 0; i < 6; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, new Color(140, 13, 13), false, 0.9f, 1, 30);
                ParticleController.SpawnParticle(particle);
            }
        }
    }
}
