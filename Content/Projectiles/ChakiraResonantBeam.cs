using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Projectiles.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class ChakiraResonantBeam : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/CursedTechniques/CursedTechnique";
        public static readonly int FRAMES = 60;
        public static readonly int TICKS_PER_FRAME = 1;

        private Texture2D texture;
        private ref float prog => ref Projectile.ai[0];
        private const float MAX_LENGTH = 2000f;
        private const float STEP_SIZE = 150f;

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.ArmorPenetration = 7;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 30;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= FRAMES)
                {
                    Projectile.frame = 0;
                }
            }

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
            Projectile.ai[1] = beamLength;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Projectiles/ChakiraResonantBeam/{Projectile.frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float beamLength = Projectile.ai[1];
            beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);

            Vector2 dir = Projectile.rotation.ToRotationVector2();
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 origin = new Vector2(0, texture.Height / 2f);

            for (float i = 0f; i < beamLength; i += STEP_SIZE)
            {
                Main.EntitySpriteDraw(texture, start + dir * i, null, Color.White, Projectile.rotation, origin, 2f, SpriteEffects.None);
            }

            return false;
        }
    }
}
