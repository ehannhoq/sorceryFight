using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.TenShadows
{
    public class ToadTongue : ModProjectile
    {
        private const int BEAM_FRAMES = 1;
        private const int TIP_FRAMES = 1;
        private const int TICKS_PER_FRAME = 5;
        private const float MAX_LENGTH = 800f;
        private const float STEP_SIZE = 4f;
        private const float BASE_BEAM_HEIGHT = 0.8f;
        private const float AIM_LERP_SPEED = 0.12f;
        private const int SLOW_DURATION = 120;


        // tongue extend / retract timings
        private const float EXTEND_SPEED = 35f;
        private const float RETRACT_SPEED = 45f;
        private const float HOLD_TICKS = 15f;
        private const float PAUSE_TICKS = 30f;

        private const float SIZE_RAMP_SPEED = 0.15f;

        // 0 = extending, 1 = holding, 2 = retracting, 3 = pausing
        private int lashState = 0;
        private float currentLength = 0f;
        private float targetLength = 0f;
        private float stateTimer = 0f;

        private int beamFrame = 0;
        private int tipFrame = 0;
        private int frameTime = 0;

        public static Texture2D beamTexture;
        public static Texture2D tipTexture;

        ref float beamHeight => ref Projectile.ai[0];
        ref float parentIndex => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            beamTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/ToadTongue", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            tipTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/ToadTongueTip", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            //Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            beamHeight = 0f;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }

        public override void AI()
        {
            int parentIdx = (int)parentIndex;
            if (!Main.projectile.IndexInRange(parentIdx) ||
                !Main.projectile[parentIdx].active ||
                Main.projectile[parentIdx].type != ModContent.ProjectileType<Toad>())
            {
                beamHeight -= 0.3f;
                if (beamHeight <= 0f)
                    Projectile.Kill();
                return;
            }

            Projectile parent = Main.projectile[parentIdx];

            // attach to the frog
            // the frog is still affected by gravity so might be moved (?)
            Projectile.Center = parent.Center + new Vector2(parent.spriteDirection * (parent.width / 3f), -parent.height / 4f + 10f);
            Projectile.timeLeft = 2;

            // TODO: change other beam ramps to be like this with a constant instead of magic number
            if (beamHeight < 1.5f)
                beamHeight += SIZE_RAMP_SPEED;

            if (frameTime++ > TICKS_PER_FRAME)
            {
                frameTime = 0;
                if (beamFrame++ >= BEAM_FRAMES - 1) beamFrame = 0;
                if (tipFrame++ >= TIP_FRAMES - 1) tipFrame = 0;
            }

            // targeting code has to handle tongue retraction / extension
            if (Main.myPlayer == Projectile.owner)
            {
                NPC target = null;
                if (parent.ModProjectile is Toad toad)
                {
                    toad.SetTarget();
                    target = toad.Target;
                }

                if (target != null)
                {
                    float targetRotation = (target.Center - Projectile.Center).ToRotation();
                    Projectile.rotation = SFUtils.LerpAngle(Projectile.rotation, targetRotation, AIM_LERP_SPEED);
                    targetLength = MathHelper.Clamp(Vector2.Distance(Projectile.Center, target.Center), 0f, MAX_LENGTH);
                }
                else
                {
                    beamHeight -= 0.3f;
                    if (beamHeight <= 0f)
                        Projectile.Kill();
                    return;
                }

                //switch tongue state
                switch (lashState)
                {
                    case 0: // extend
                        currentLength += EXTEND_SPEED;
                        if (currentLength >= targetLength)
                        {
                            currentLength = targetLength;
                            lashState = 1;
                            stateTimer = 0f;
                        }
                        break;

                    case 1: // hold target
                        stateTimer++;
                        if (stateTimer >= HOLD_TICKS)
                        {
                            lashState = 2;
                        }
                        break;

                    case 2: // retracting
                        currentLength -= RETRACT_SPEED;
                        if (currentLength <= 0f)
                        {
                            currentLength = 0f;
                            lashState = 3;
                            stateTimer = 0f;
                        }
                        break;

                    case 3: // pausing
                        stateTimer++;
                        if (stateTimer >= PAUSE_TICKS)
                        {
                            lashState = 0;
                        }
                        break;
                }

                Projectile.localAI[0] = currentLength;
                Projectile.netUpdate = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, SLOW_DURATION);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (beamTexture == null || tipTexture == null)
                return false;

            float drawLength = Projectile.localAI[0];

            if (drawLength < 10f)
                return false;

            Vector2 beamStart = Projectile.Center - Main.screenPosition;
            Vector2 beamScale = new Vector2(drawLength / beamTexture.Width, BASE_BEAM_HEIGHT * beamHeight);

            // Draw tongue body
            int beamFrameHeight = beamTexture.Height / BEAM_FRAMES;
            int beamFrameY = beamFrame * beamFrameHeight;
            Vector2 beamOrigin = new Vector2(0, beamFrameHeight / 2);
            Rectangle beamSourceRectangle = new Rectangle(0, beamFrameY, beamTexture.Width, beamFrameHeight);
            Main.EntitySpriteDraw(beamTexture, beamStart, beamSourceRectangle, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0f);

            // Draw tongue tip
            int tipFrameHeight = tipTexture.Height / TIP_FRAMES;
            int tipFrameY = tipFrame * tipFrameHeight;
            Vector2 tipPos = beamStart + Projectile.rotation.ToRotationVector2() * drawLength;
            Vector2 tipOrigin = new Vector2(tipTexture.Width / 2, tipFrameHeight / 2);
            Rectangle tipSourceRectangle = new Rectangle(0, tipFrameY, tipTexture.Width, tipFrameHeight);
            Main.EntitySpriteDraw(tipTexture, tipPos, tipSourceRectangle, Color.White, Projectile.rotation, tipOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);

            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float length = Projectile.localAI[0];
            if (length < 10f)
                return false;

            //TODO: have been copy pasting this collision code a lot, should make this variable name less generic, something like uselessCollisionPoint
            float useless = 0f;

            Vector2 tongueEnd = Projectile.Center + Projectile.rotation.ToRotationVector2() * length;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, tongueEnd, beamHeight * Projectile.scale, ref useless))
                return true;

            return false;
        }
    }
}
