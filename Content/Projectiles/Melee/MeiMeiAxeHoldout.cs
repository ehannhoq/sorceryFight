using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.VFX;
using sorceryFight.Utilities.EaseFunctions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.Melee
{
    public class MeiMeiAxeHoldout : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/Items/Weapons/Melee/MeiMeiAxe";

        private ref float topRotation => ref Projectile.ai[0];
        private ref float bottomRotation => ref Projectile.ai[1];

        private const float ROTATION_OFFSET = 3 * MathHelper.PiOver4;

        private const int lifetime = 60;
        private const int swingTime = 40;
        private const int postSwingDelay = 10;
        public override void SetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 115;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.direction = (Math.Cos(Projectile.velocity.ToRotation()) > 0).ToDirectionInt();
            player.ChangeDir(Projectile.direction);

            float initialRotation = Projectile.velocity.ToRotation();
            topRotation = initialRotation - ROTATION_OFFSET * Projectile.direction;
            bottomRotation = initialRotation + ROTATION_OFFSET * Projectile.direction;
            Projectile.rotation = topRotation;

            Projectile.Center = player.Center + (Vector2.UnitX * Projectile.width).RotatedBy(Projectile.rotation);
            Projectile.timeLeft = lifetime + postSwingDelay;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.direction = Projectile.direction;
            float angle;
            float progress;
            float overshoot = 0.5f * Projectile.direction;

            int timeSinceSpawn = lifetime + postSwingDelay - Projectile.timeLeft;

            if (timeSinceSpawn < swingTime / 4)
            {
                progress = EaseFunctions.EaseOut(timeSinceSpawn / (float)(swingTime / 4));
                angle = MathHelper.Lerp(topRotation, topRotation - overshoot, progress);
            }
            else if (timeSinceSpawn < swingTime - swingTime / 4)
            {
                progress = EaseFunctions.EaseInOut((timeSinceSpawn - swingTime / 4) / (float)(swingTime - swingTime / 2));
                angle = MathHelper.Lerp(topRotation - overshoot, bottomRotation + overshoot, progress);
            }
            else if (timeSinceSpawn < swingTime)
            {
                progress = EaseFunctions.EaseOut((timeSinceSpawn - (swingTime - swingTime / 4)) / (float)(swingTime / 4));
                angle = MathHelper.Lerp(bottomRotation + overshoot, bottomRotation, progress);
            }
            else
            {
                angle = bottomRotation;
            }

            Projectile.rotation = angle;
            Projectile.Center = player.Center + (Vector2.UnitX * Projectile.width).RotatedBy(Projectile.rotation);
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
            VFXManager.AddVFX(new ImpactCircleVFX(
                center: target.Center,
                lifetime: 30
            ));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);

            SpriteEffects flip = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.rotation + MathHelper.PiOver4 * Projectile.direction, src.Size() * 0.5f, Projectile.scale, flip);
            return false;
        }
    }
}