using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.CameraModifiers;
using Terraria.DataStructures;
using Terraria.Audio;
using Terraria.ID;

namespace sorceryFight.Content.NPCs.Bosses.TenShadows.RabbitEscape
{
    public class RabbitLaser : ModProjectile
    {
        public static Texture2D texture;
        public static Texture2D convergenceTexture;

        private const float MAX_LENGTH = 1600f;
        private const float STEP_SIZE = 4f;
        private const float BASE_BEAM_HEIGHT = 0.5f;

        ref float beamHeight => ref Projectile.ai[0];

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitLaser", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            convergenceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitLaser_Convergence", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SoundID.Zombie104);
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 7.5f, 3f, 210, 1000f));
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.damage = 150;
            beamHeight = 0f;
        }

        public override void AI()
        {
            if (Projectile.ai[1]++ < 180f)
            {
                float t = Projectile.ai[1] / 180f; 
                t = 1f - (float)Math.Pow(1f - t, 3);
                Projectile.rotation = MathHelper.Lerp(MathHelper.ToRadians(0), MathHelper.ToRadians(180), t);
            }
            if (beamHeight < 2.0f)
                beamHeight += 0.2f;
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

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            if(texture == null || !Main.dedServ)
            {
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitLaser", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                convergenceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitLaser_Convergence", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
            float beamLength = Projectile.localAI[0] - 50f;
            beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);
            Vector2 beamScale = new Vector2((beamLength - convergenceTexture.Width / 2) / texture.Width, BASE_BEAM_HEIGHT * beamHeight);
            Vector2 beamOrigin = new Vector2(0, texture.Height / 2);
            float drawn = convergenceTexture.Width / 2f; //this is the convergence length without the gap if anyone sees this
            while (drawn < beamLength)
            {
                float segmentLength = Math.Min(texture.Width, beamLength - drawn);
                Vector2 drawPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * drawn - Main.screenPosition;
                Rectangle sourceRect = new Rectangle(0, 0, (int)segmentLength, texture.Height);
                Main.EntitySpriteDraw(texture, drawPos, sourceRect, Color.White, 
                    Projectile.rotation, beamOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);

                drawn += texture.Width;
            }

            Vector2 convergenceOrigin = new Vector2(convergenceTexture.Width / 2, convergenceTexture.Height / 2);
            Rectangle convergenceSourceRectangle = new Rectangle(0, 0, convergenceTexture.Width, convergenceTexture.Height);
            Main.EntitySpriteDraw(convergenceTexture, Projectile.Center - Main.screenPosition, convergenceSourceRectangle, Color.White, Projectile.rotation, convergenceOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
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
