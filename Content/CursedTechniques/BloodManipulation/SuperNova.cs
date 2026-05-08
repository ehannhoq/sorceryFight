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
using sorceryFight.Content.Particles.UIParticles;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class SuperNova : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 3;
        public static readonly int TICKS_PER_FRAME = 5;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.SuperNova.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.SuperNova.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.SuperNova.LockedDescription");
        public override float Cost => 40f;

        public override float BloodCost => 100f;

        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;

        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 100;

        public override float Speed => 25f;
        public override float LifeTime => 120f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            // return sf.HasDefeatedBoss(ModContent.NPCType<AstrumDeusHead>());
            return true;
        }


        public static Texture2D texture;

        public bool animating;
        public float animScale;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<SuperNova>();
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            sf.bloodEnergy -= BloodCost;
            sf.cursedEnergy -= CalculateTrueCost(sf);

            if (Main.myPlayer == player.whoAmI)
            {
                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }

                Vector2 mousePos = Main.MouseWorld;
                var entitySource = player.GetSource_FromThis();
                int index = Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, GetProjectileType(), (int)CalculateTrueDamage(sf), 0f, player.whoAmI);
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
            animating = false;
            Projectile.penetrate = -1;
            animScale = 1.25f;
            Projectile.timeLeft = (int)LifeTime;
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

            if (Vector2.Distance(Projectile.Center, targetPos) > Speed)
            {
                Projectile.velocity = Projectile.DirectionTo(targetPos) * Speed;
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
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

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

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, textColor, false, 0.9f, 1, 30);
                ParticleController.SpawnParticle(particle);
            }
        }

    }
}