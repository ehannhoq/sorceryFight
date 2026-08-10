using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using sorceryFight.SFPlayer;
using System;
using sorceryFight.Utilities.EaseFunctions;
using ReLogic.Reflection;
using sorceryFight.Content.VFX;
using sorceryFight.Content.Particles;


namespace sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain
{
    public class RailroadSign : CursedTechnique
    {
        public static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/RailroadSign", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public override string InternalName => "RailroadSign";

        public Player Owner => Main.player[Projectile.owner];
        private ref float topRotation => ref Projectile.ai[0];
        private ref float bottomRotation => ref Projectile.ai[1];
        private ref float initialDir => ref Projectile.ai[2];

        private const float ROTATION_OFFSET = 3 * MathHelper.PiOver4;

        private const int swingTime = 15;
        private const int windupTime = 20;
        private const int windDownTime = 10;

        public RailroadSign()
        {
            Technique.baseDamage = 150;
            Technique.damagePerBoss = 50;
            Technique.cost = 90;
            Technique.lifetime = swingTime + windupTime + windDownTime;
            Technique.speed = 24f;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 70;
            Projectile.height = 230;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (i == Projectile.whoAmI)
                    continue;

                Projectile proj = Main.projectile[i];

                if (proj.type == ModContent.ProjectileType<RailroadSign>() && proj.owner == Projectile.owner)
                {
                    proj.Kill();
                }
            }

            Player player = Main.player[Projectile.owner];
            Projectile.direction = (Math.Cos(Projectile.velocity.ToRotation()) > 0).ToDirectionInt();
            player.ChangeDir(Projectile.direction);

            float initialRotation = Projectile.velocity.ToRotation();
            if (Projectile.direction == -1 && initialRotation < 0)
                initialRotation += MathHelper.TwoPi;
            topRotation = initialRotation - ROTATION_OFFSET * Projectile.direction;
            bottomRotation = initialRotation + ROTATION_OFFSET * Projectile.direction;
            initialDir = Projectile.direction;

            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = topRotation;
            Projectile.Center = player.Center + (Vector2.UnitX * Projectile.height * 0.5f).RotatedBy(Projectile.rotation);
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.SorceryFight().disableRegenFromProjectiles = true;

            player.direction = (int)initialDir;
            Projectile.direction = (int)initialDir;
            float angle;
            float overshoot = 0.5f * Projectile.direction;


            int timeSinceSpawn = lifetime - Projectile.timeLeft;

            if (timeSinceSpawn == windupTime + (swingTime / 4))
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, player.Center);

            if (timeSinceSpawn < windupTime)
            {
                float percent = timeSinceSpawn / (float)windupTime;
                float progress = EaseFunctions.EaseOut(percent);
                angle = MathHelper.Lerp(topRotation, topRotation - overshoot, progress);
            }
            else if (timeSinceSpawn < windupTime + swingTime)
            {
                float percent = (timeSinceSpawn - windupTime) / (float)swingTime;
                float progress = EaseFunctions.EaseInOut(percent);
                angle = MathHelper.Lerp(topRotation - overshoot, bottomRotation + overshoot, progress);
            }
            else if (timeSinceSpawn < windupTime + swingTime + windDownTime)
            {
                float percent = (timeSinceSpawn - windupTime - swingTime) / (float)windDownTime;
                float progress = EaseFunctions.EaseOut(percent);
                angle = MathHelper.Lerp(bottomRotation + overshoot, bottomRotation, progress);
            }
            else
            {
                angle = bottomRotation;
            }

            Projectile.rotation = angle;
            Projectile.Center = player.Center + (Vector2.UnitX * Projectile.height * 0.5f).RotatedBy(Projectile.rotation);
            
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
            player.itemRotation = Projectile.rotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.rotation + MathHelper.PiOver2, src.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 center = Main.player[Projectile.owner].Center;
            Vector2 end = center + (Vector2.UnitX * Projectile.height).RotatedBy(Projectile.rotation);
            float lineWidth = Projectile.width;
            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), center, end, lineWidth, ref _);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.velocity = Vector2.Zero;

            VFXManager.AddVFX(new ImpactCircleVFX(
                center: target.Center,
                lifetime: 30,
                scale: 1.5f
            ));

            for (int i = 0; i < 3; i++)
            {
                StarParticle particle = new StarParticle(
                    position: target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f),
                    velocity: Vector2.Zero,
                    color: Color.White,
                    changeOpacity: true,
                    lifetime: 15,
                    scale: 1f
                );
                ParticleController.SpawnParticle(particle);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.SorceryFight().disableRegenFromProjectiles = false;
        }
    }
}