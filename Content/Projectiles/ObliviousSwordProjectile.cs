using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class ObliviousSwordProjectile : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/CursedTechniques/CursedTechnique";
        private Texture2D texture;
        private const int FRAMES = 16;
        private const int TICKS_PER_FRAME = 2;
        private int target = -1;
        private float trackingRadius = 250f;

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 50;
            Projectile.penetrate = -1;
            Projectile.knockBack = 0;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame++ >= FRAMES - 1)
                    Projectile.frame = 0;
            }


            if (target == -1)
                FindTarget();
            else if (target >= 0 && !Main.npc[target].active)
                FindTarget();
            else if (target >= 0)
            {
                Vector2 targetVelocity = (Main.npc[target].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.25f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        void FindTarget()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && Vector2.DistanceSquared(npc.Center, Projectile.Center) < trackingRadius.Squared())
                {
                    target = npc.whoAmI;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            this.target = -2;
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

            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Projectiles/ObliviousSwordProjectile/{Projectile.frame}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, projOrigin, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }
    }
}