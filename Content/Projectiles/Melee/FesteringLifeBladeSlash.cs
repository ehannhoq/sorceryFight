using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;

using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.Melee
{
    public class FesteringLifeBladeSlash : ModProjectile
    {
        private static Texture2D texture;
        private const int FRAME_COUNT = 9;
        private const int TICKS_PER_FRAME = 2;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Melee/FesteringLifeBladeSlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 70;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
            Projectile.scale = 0.75f;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.PositionProjectileForSlash(35f);
        }

        public override void AI()
        {
            Projectile.HandleProjectileAnimation(FRAME_COUNT, TICKS_PER_FRAME);
            Projectile.PositionProjectileForSlash(35f);

            Player player = Main.player[Projectile.owner];
            float swingProgress = (float)Projectile.frame / (FRAME_COUNT - 1);
            float armRotation = MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, swingProgress);

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, (armRotation - MathHelper.PiOver2) * player.direction);
            player.itemRotation = armRotation;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, projOrigin, Projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
