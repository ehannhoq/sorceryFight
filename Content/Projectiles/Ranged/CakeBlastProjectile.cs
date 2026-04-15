using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.Ranged
{
    public class CakeBlastProjectile : ModProjectile
    {
        private const int CONVERGENCE_FRAMES = 4;
        private const int BEAM_FRAMES = 4;
        private const int TICKS_PER_FRAME = 4;
        private int convergenceFrame = 0;
        private int beamFrame = 0;
        private int frameTime = 0;
        public static Texture2D texture;
        public static Texture2D convergenceTexture;

        private const float MAX_LENGTH = 1600f;
        private const float STEP_SIZE = 4f;
        private const float BASE_BEAM_HEIGHT = 0.5f;

        ref float beamHeight => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Ranged/CakeBlastProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            convergenceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Ranged/CakeConvergence", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            beamHeight = 0f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // kill if player stops channeling or runs out of mana
            if (!player.channel || player.dead || !player.active)
            {
                beamHeight -= 0.2f;
                if (beamHeight <= 0f)
                    Projectile.Kill();
                return;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                float targetRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.rotation = SFUtils.LerpAngle(Projectile.rotation, targetRotation, 0.2f);
                Projectile.direction = Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1;
                player.ChangeDir(Projectile.direction);
                Projectile.netUpdate = true;
            }

            Projectile.Center = player.Center;
            Projectile.timeLeft = 2; // keep alive while channeling

            if (beamHeight < 2.0f)
                beamHeight += 0.2f;

            if (frameTime++ > TICKS_PER_FRAME)
            {
                frameTime = 0;
                if (convergenceFrame++ >= CONVERGENCE_FRAMES - 1) convergenceFrame = 0;
                if (beamFrame++ >= BEAM_FRAMES - 1) beamFrame = 0;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                float beamLength = 0f;
                Vector2 direction = Projectile.rotation.ToRotationVector2();
                for (float i = 0f; i < MAX_LENGTH; i += STEP_SIZE)
                {
                    Vector2 checkPos = Projectile.Center + direction * i;
                    if (!Collision.CanHitLine(Projectile.Center, 1, 1, checkPos, 1, 1))
                        break;
                    beamLength = i;
                }
                Projectile.localAI[0] = beamLength;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float beamLength = Projectile.localAI[0] - 50f;
            beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);

            Vector2 beamStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 2 * (convergenceTexture.Width / 2) - Main.screenPosition;
            Vector2 beamScale = new Vector2((beamLength - convergenceTexture.Width / 2) / texture.Width, BASE_BEAM_HEIGHT * beamHeight);

            int beamFrameHeight = texture.Height / BEAM_FRAMES;
            int beamFrameY = beamFrame * beamFrameHeight;
            Vector2 beamOrigin = new Vector2(0, beamFrameHeight / 2);
            Rectangle beamSourceRectangle = new Rectangle(0, beamFrameY, texture.Width, beamFrameHeight);
            Main.EntitySpriteDraw(texture, beamStart, beamSourceRectangle, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0f);

            int convFrameHeight = convergenceTexture.Height / CONVERGENCE_FRAMES;
            int convFrameY = convergenceFrame * convFrameHeight;
            Vector2 convergenceOrigin = new Vector2(convergenceTexture.Width / 2, convFrameHeight / 2);
            Rectangle convergenceSourceRectangle = new Rectangle(0, convFrameY, convergenceTexture.Width, convFrameHeight);
            Main.EntitySpriteDraw(convergenceTexture, beamStart, convergenceSourceRectangle, Color.White, Projectile.rotation, convergenceOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);

            return false;
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
