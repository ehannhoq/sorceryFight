using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.UI.CursedTechniqueMenu;
using sorceryFight.SFPlayer;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;


namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class PiercingBlood : CursedTechniqueContinuous
    {
        public static Texture2D texture;
        public static Texture2D convergenceTexture;
        public static Texture2D collisionTexture;

        public override string InternalName => "PiercingBlood";

        private const int CONVERGENCE_FRAMES = 5;
        private const int COLLISION_FRAMES = 5;
        private const int TICKS_PER_FRAME = 5;
        private int convergenceFrame = 0;
        private int collisionFrame = 0;
        private int frameTime = 0;
        private const float MAX_LENGTH = 1600f;
        private const float STEP_SIZE = 4f;
        private const float BASE_BEAM_HEIGHT = 0.5f;
        ref float justSpawned => ref Projectile.ai[0];
        ref float beamHeight => ref Projectile.ai[1];

        public PiercingBlood()
        {
            Technique.baseDamage = 7;
            Technique.damagePerBoss = 7;
            Technique.cost = 50;
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string localizationCategoryKey = "Mods.sorceryFight.Misc.CursedTechniques";

            string damage = SFUtils.GetLocalization(localizationCategoryKey + ".Damage")
                .WithFormatArgs(CalculateTrueDamage(sf)).Value;

            string ceCost = SFUtils.GetLocalization(localizationCategoryKey + ".ContinuousCost")
                .WithFormatArgs((int)base.CalculateTrueCost(sf)).Value;

            string bloodCost = SFUtils.GetLocalization(localizationCategoryKey + ".ContinuousBloodCost")
                .WithFormatArgs((int)Technique.cost / 2).Value;

            string stats = damage + "\n" + ceCost + "\n" + bloodCost;

            return stats;
        }

        public override void DrainCost(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.cursedEnergy -= CalculateTrueCost(sfPlayer);
            sfPlayer.bloodEnergy -= SFUtils.RateSecondsToTicks(Technique.cost / 2);
            if (sfPlayer.bloodEnergy <= 1)
                Destroy(sfPlayer);
        }


        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/PiercingBlood", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            convergenceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/Convergence", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            collisionTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/PiercingBloodCollision", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            beamHeight = 0.0f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.rotation = (Main.MouseWorld - Main.LocalPlayer.Center).ToRotation();
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();

            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.Center = player.Center;

                float targetRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.rotation = SFUtils.LerpAngle(Projectile.rotation, targetRotation, 0.2f);
                Projectile.direction = Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1;
                player.ChangeDir(Projectile.direction);
                Projectile.netUpdate = true;
            }


            if (frameTime++ > TICKS_PER_FRAME)
            {
                frameTime = 0;
                if (convergenceFrame++ >= CONVERGENCE_FRAMES - 1)
                {
                    convergenceFrame = CONVERGENCE_FRAMES - 1;
                }
                if (collisionFrame++ >= COLLISION_FRAMES - 1)
                {
                    collisionFrame = 0;
                }
            }

            if (convergenceFrame != CONVERGENCE_FRAMES - 1) return;

            if (justSpawned == 0f)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (i == Projectile.whoAmI)
                        continue;

                    Projectile proj = Main.projectile[i];

                    if (proj.type == ModContent.ProjectileType<PiercingBlood>() && proj.owner == Projectile.owner)
                    {
                        proj.Kill();
                    }
                }
                justSpawned = 1f;
                SoundEngine.PlaySound(SorceryFightSounds.PiercingBlood, Projectile.Center);
            }

            if (beamHeight < 2.0f && keyHeld && sfPlayer.bloodEnergy > 1)
                beamHeight += 0.2f;


            float beamLength = 0f;
            Vector2 direction = Projectile.rotation.ToRotationVector2();
            for (float i = 0f; i < MAX_LENGTH; i += STEP_SIZE)
            {
                Vector2 checkPos = Projectile.Center + direction * i;
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, checkPos, 1, 1))
                {
                    break;
                }
                beamLength = i;
            }
            Projectile.localAI[0] = beamLength;
        }


        public override void Destroy(SorceryFightPlayer sfPlayer)
        {
            beamHeight -= 0.2f;
            sfPlayer.disableRegenFromProjectiles = false;
            if (beamHeight <= 0f)
                Projectile.Kill();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            float beamLength = Projectile.localAI[0] - 50f;
            beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);


            Vector2 beamStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 2 * (convergenceTexture.Width / 2) - Main.screenPosition;
            Vector2 beamOrigin = new Vector2(0, texture.Height / 2);
            Vector2 beamScale = new Vector2((beamLength - convergenceTexture.Width / 2) / texture.Width, BASE_BEAM_HEIGHT * beamHeight);

            Main.EntitySpriteDraw(texture, beamStart, null, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0f);


            int convFrameHeight = convergenceTexture.Height / CONVERGENCE_FRAMES;
            int convFrameY = convergenceFrame * convFrameHeight;

            Vector2 convergenceOrigin = new Vector2(convergenceTexture.Width / 2, convFrameHeight / 2);
            Rectangle convergenceSourceRectangle = new Rectangle(0, convFrameY, convergenceTexture.Width, convFrameHeight);

            Main.EntitySpriteDraw(convergenceTexture, beamStart, convergenceSourceRectangle, Color.White, Projectile.rotation, convergenceOrigin, 2f, SpriteEffects.None, 0f);

            int collisionFrameHeight = collisionTexture.Height / COLLISION_FRAMES;
            int collisionFrameY = collisionFrame * collisionFrameHeight;

            if (beamLength > 20f)
            {
                Vector2 beamEnd = beamStart + Projectile.rotation.ToRotationVector2() * beamLength;

                Vector2 collisionOrigin = new Vector2(collisionTexture.Width / 2, collisionFrameHeight / 2);
                Rectangle collisionSourceRectangle = new Rectangle(0, collisionFrameY, collisionTexture.Width, collisionFrameHeight);

                Main.EntitySpriteDraw(collisionTexture, beamEnd, collisionSourceRectangle, Color.White, Projectile.rotation, collisionOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);
            }


            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            int paintingCount = Main.player[Projectile.owner].SorceryFight().deathPaintings.Count(p => p);
            target.AddBuff(ModContent.BuffType<BloodPoison>(), paintingCount * 60);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0], beamHeight * Projectile.scale, ref useless))
                return true;

            return false;
        }
    }
}
