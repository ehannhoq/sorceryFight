using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.TenShadows
{
    public class MaxElephantBeam : ModProjectile
    {
        private const int CONVERGENCE_FRAMES = 5;
        private const int BEAM_FRAMES = 5;
        private const int COLLISION_FRAMES = 5;
        private const int TICKS_PER_FRAME = 5;
        private const float MAX_LENGTH = 1600f;
        private const float STEP_SIZE = 4f;
        private const float BASE_BEAM_HEIGHT = 0.5f;
        private const float AIM_LERP_SPEED = 0.08f;

        private int beamFrame = 0;
        private int convergenceFrame = 0;
        private int collisionFrame = 0;
        private int frameTime = 0;

        public static Texture2D beamTexture;
        public static Texture2D convergenceTexture;
        public static Texture2D collisionTexture;

        ref float beamHeight => ref Projectile.ai[0];
        ref float parentIndex => ref Projectile.ai[1];

        public float animScale;

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            beamTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/MaxElephantBeam", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            convergenceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/MaxElephantBeamConvergence", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            collisionTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/MaxElephantBeamCollision", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            beamHeight = 0f;
        }

        public override void AI()
        {
            int parentIdx = (int)parentIndex;
            if (!Main.projectile.IndexInRange(parentIdx) ||
                !Main.projectile[parentIdx].active ||
                Main.projectile[parentIdx].type != ModContent.ProjectileType<MaxElephant>())
            {
                beamHeight -= 0.2f;
                if (beamHeight <= 0f)
                    Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[parentIdx];

            Projectile.Center = parent.Center + new Vector2(0f, -parent.height / 2f);
            Projectile.timeLeft = 2;

            if (beamHeight < 2.0f)
                beamHeight += 0.1f;

            if (frameTime++ > TICKS_PER_FRAME)
            {
                frameTime = 0;
                if (convergenceFrame++ >= CONVERGENCE_FRAMES - 1) convergenceFrame = 0;
                if (beamFrame++ >= BEAM_FRAMES - 1) beamFrame = 0;
                if (collisionFrame++ >= COLLISION_FRAMES - 1) collisionFrame = 0;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                NPC target = null;
                if (parent.ModProjectile is MaxElephant elephant)
                {
                    elephant.SetTarget();
                    target = elephant.Target;
                }

                if (target != null)
                {
                    float targetRotation = (target.Center - Projectile.Center).ToRotation();
                    Projectile.rotation = SFUtils.LerpAngle(Projectile.rotation, targetRotation, AIM_LERP_SPEED);
                }
                else
                {
                    beamHeight -= 0.2f;
                    if (beamHeight <= 0f)
                        Projectile.Kill();
                    return;
                }

               
                float beamLength = 0f;
                Vector2 direction = Projectile.rotation.ToRotationVector2();

                //beam start area is 20 pixels up
                Vector2 startPos = Projectile.Center + direction * 20f;

                for (float i = 0f; i < MAX_LENGTH; i += STEP_SIZE)
                {
                    Vector2 checkPos = startPos + direction * i;
                    if (!Collision.CanHitLine(startPos, 1, 1, checkPos, 1, 1))
                        break;
                    beamLength = i;
                }
                Projectile.localAI[0] = beamLength;

                Projectile.netUpdate = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamTexture == null || convergenceTexture == null || collisionTexture == null)
                return false;

            float beamLength = Projectile.localAI[0] - 50f;
            beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);

            if (beamLength < 20f)
                return false;

            Vector2 beamStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 2 * (convergenceTexture.Width / 2) - Main.screenPosition;
            Vector2 beamScale = new Vector2((beamLength - convergenceTexture.Width / 2) / beamTexture.Width, BASE_BEAM_HEIGHT * beamHeight);

            // beam main
            int beamFrameHeight = beamTexture.Height / BEAM_FRAMES;
            int beamFrameY = beamFrame * beamFrameHeight;
            Vector2 beamOrigin = new Vector2(0, beamFrameHeight / 2);
            Rectangle beamSourceRectangle = new Rectangle(0, beamFrameY, beamTexture.Width, beamFrameHeight);
            Main.EntitySpriteDraw(beamTexture, beamStart, beamSourceRectangle, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0f);

            // convergence
            int convFrameHeight = convergenceTexture.Height / CONVERGENCE_FRAMES;
            int convFrameY = convergenceFrame * convFrameHeight;
            Vector2 convergenceOrigin = new Vector2(convergenceTexture.Width / 2, convFrameHeight / 2);
            Rectangle convergenceSourceRectangle = new Rectangle(0, convFrameY, convergenceTexture.Width, convFrameHeight);
            Main.EntitySpriteDraw(convergenceTexture, beamStart, convergenceSourceRectangle, Color.White, Projectile.rotation, convergenceOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);

            // collison
            int collisionFrameHeight = collisionTexture.Height / COLLISION_FRAMES;
            int collisionFrameY = collisionFrame * collisionFrameHeight;
            Vector2 beamEnd = beamStart + Projectile.rotation.ToRotationVector2() * (beamLength - 25);
            Vector2 collisionOrigin = new Vector2(collisionTexture.Width / 2, collisionFrameHeight / 2);
            Rectangle collisionSourceRectangle = new Rectangle(0, collisionFrameY, collisionTexture.Width, collisionFrameHeight);
            Main.EntitySpriteDraw(collisionTexture, beamEnd, collisionSourceRectangle, Color.White, Projectile.rotation, collisionOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float useless = 0f;
            Vector2 beamEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0];
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEnd, beamHeight * Projectile.scale, ref useless))
                return true;

            return false;
        }
    }
}
