using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class MassKick : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 12;
        public static readonly int TICKS_PER_FRAME = 3;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.MassKick.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.MassKick.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.MassKick.LockedDescription");
        public override float Cost => 40f;

        public override float BloodCost => 20f;

        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;

        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 0f;
        public override float LifeTime => 36f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }

        //this is only here to try and set velocity to zero to make the projecitle not move based on mouse position
        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                var entitySource = player.GetSource_FromThis();

                sf.cursedEnergy -= CalculateTrueCost(sf);

                if (BloodCost > 0)
                    sf.bloodEnergy -= BloodCost;

                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }

                return Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, GetProjectileType(), (int)CalculateTrueDamage(sf), 0, player.whoAmI);
            }
            return -1;
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
            return ModContent.ProjectileType<MassKick>();
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 65;
            Projectile.height = 65;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            animScale = 2;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Player player = Main.player[Projectile.owner];

            Projectile.ai[0]++;
            float progress = Projectile.ai[0] / LifeTime;

            float dashStart = 0.4f;
            float dashEnd = 0.8f;

            //make animation starting frames faster
            int ticksThisFrame;
            if (progress < dashStart)
                ticksThisFrame = 3;
            else
                ticksThisFrame = TICKS_PER_FRAME;

            if (progress >= dashStart && progress < dashEnd)
            {
                float dashProgress = (progress - dashStart) / (dashEnd - dashStart);

                //iframes
                player.immune = true;
                int remainingDashTicks = (int)((dashEnd - progress) * LifeTime);
                player.immuneTime = remainingDashTicks;
                for (int i = 0; i < player.hurtCooldowns.Length; i++)
                {
                    player.hurtCooldowns[i] = remainingDashTicks;
                }

                player.velocity.X += MathHelper.Lerp(1.5f, 0f, dashProgress) * player.direction;
            }

            if (progress >= 1f)
                Projectile.Kill();

            //float xOffset = MathHelper.Lerp(-100f, 100f, progress) * player.direction;
            float xOffset = MathHelper.Lerp(-60f, 60f, progress) * player.direction;
            Projectile.Center = player.Center + new Vector2(xOffset, 0f);

            if (Projectile.ai[0] > LifeTime)
            {
                Projectile.Kill();
            }


            if (Projectile.frameCounter++ >= ticksThisFrame)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> alreadyDrawnProjectiles)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/StarRage/MassKick").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Player player = Main.player[Projectile.owner];
            SpriteEffects effects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, effects, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 6; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, textColor, false, 0.9f, 1f, 30);
                ParticleController.SpawnParticle(particle);
            }
        }

    }
}