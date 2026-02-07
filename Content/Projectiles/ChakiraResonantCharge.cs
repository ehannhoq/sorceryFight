using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class ChakiraResonantCharge : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/CursedTechniques/CursedTechnique";
        public static readonly int FRAMES = 60;
        public static readonly int TICKS_PER_FRAME = 1;

        private Texture2D texture;

        private ref float prog => ref Projectile.ai[0];

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

            if (Projectile.frame % 30 == 1)
                SoundEngine.PlaySound(SorceryFightSounds.ChakiraResonantProjectileAmbiance, Projectile.Center);
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

            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Projectiles/ChakiraResonantCharge/{Projectile.frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, projOrigin, prog * 2f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }
    }
}
