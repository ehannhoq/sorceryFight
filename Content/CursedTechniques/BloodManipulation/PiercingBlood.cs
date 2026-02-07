using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class PiercingBlood : CursedTechnique
    {
        private const int CONVERGENCE_FRAMES = 5;
        private const int COLLISION_FRAMES = 5;
        private const int TICKS_PER_FRAME = 5;
        private int convergenceFrame = 0;
        private int collisionFrame = 0;
        private int frameTime = 0;
        public static Texture2D texture;
        public static Texture2D convergenceTexture;
        public static Texture2D collisionTexture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.PiercingBlood.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PiercingBlood.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PiercingBlood.LockedDescription");
        public override float Cost => 750f;
        public override Color textColor => new Color(132, 4, 4);
        public override bool DisplayNameInGame => true;
        public override int Damage => 100;
        public override int MasteryDamageMultiplier => 18;
        public override float Speed => 0f;
        public override float LifeTime => 240f;

        private const float MAX_LENGTH = 1600f;
        private const float STEP_SIZE = 4f;
        private const float BASE_BEAM_HEIGHT = 0.5f;
        ref float justSpawned => ref Projectile.ai[0];
        ref float beamHeight => ref Projectile.ai[1];

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<PiercingBlood>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.sukunasFingerConsumed >= 19;
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/PiercingBlood", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            convergenceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/Convergence", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            collisionTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/PiercingBloodCollision", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            beamHeight = 0.0f;
            Projectile.timeLeft = (int)LifeTime;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            int index = base.UseTechnique(sf);
            Main.projectile[index].rotation = (Main.MouseWorld - sf.Player.Center).ToRotation();
            return index;
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                Player player = Main.player[Projectile.owner];
                Projectile.Center = player.Center;

                float targetRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.rotation = SFUtils.LerpAngle(Projectile.rotation, targetRotation, 0.2f);
                Projectile.direction = Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1;
                player.ChangeDir(Projectile.direction);
            }


            if (frameTime++ > TICKS_PER_FRAME)
            {
                frameTime = 0;
                if (convergenceFrame++ >= CONVERGENCE_FRAMES - 1)
                {
                    convergenceFrame = CONVERGENCE_FRAMES - 1;
                }
                if (collisionFrame++ >= COLLISION_FRAMES - 1)
                {
                    collisionFrame = 0;
                }
            }

            if (convergenceFrame != CONVERGENCE_FRAMES - 1) return;

            if (justSpawned == 0f)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (i == Projectile.whoAmI)
                        continue;

                    Projectile proj = Main.projectile[i];

                    if (proj.type == ModContent.ProjectileType<PiercingBlood>() && proj.owner == Projectile.owner)
                    {
                        proj.Kill();
                    }
                }
                justSpawned = 1f;
                Main.player[Projectile.owner].SorceryFight().disableRegenFromProjectiles = true;
                SoundEngine.PlaySound(SorceryFightSounds.PiercingBlood, Projectile.Center);
            }

            if (beamHeight < 2.0f && Projectile.timeLeft > 10)
                beamHeight += 0.2f;

            if (Projectile.timeLeft <= 10)
            {
                beamHeight -= 0.2f;
                Main.player[Projectile.owner].SorceryFight().disableRegenFromProjectiles = false;
            }

            if (Main.myPlayer == Projectile.owner)
            {
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
                Projectile.localAI[0] = beamLength;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float beamLength = Projectile.localAI[0] - 50f;
            beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);


            Vector2 beamStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 2 * (convergenceTexture.Width / 2) - Main.screenPosition;
            Vector2 beamOrigin = new Vector2(0, texture.Height / 2);
            Vector2 beamScale = new Vector2((beamLength - convergenceTexture.Width / 2) / texture.Width, BASE_BEAM_HEIGHT * beamHeight);

            Main.EntitySpriteDraw(texture, beamStart, null, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0f);


            int convFrameHeight = convergenceTexture.Height / CONVERGENCE_FRAMES;
            int convFrameY = convergenceFrame * convFrameHeight;

            Vector2 convergenceOrigin = new Vector2(convergenceTexture.Width / 2, convFrameHeight / 2);
            Rectangle convergenceSourceRectangle = new Rectangle(0, convFrameY, convergenceTexture.Width, convFrameHeight);

            Main.EntitySpriteDraw(convergenceTexture, beamStart, convergenceSourceRectangle, Color.White, Projectile.rotation, convergenceOrigin, 2f, SpriteEffects.None, 0f);

            int collisionFrameHeight = collisionTexture.Height / COLLISION_FRAMES;
            int collisionFrameY = collisionFrame * collisionFrameHeight;

            if (beamLength > 20f)
            {
                Vector2 beamEnd = beamStart + Projectile.rotation.ToRotationVector2() * beamLength;

                Vector2 collisionOrigin = new Vector2(collisionTexture.Width / 2, collisionFrameHeight / 2);
                Rectangle collisionSourceRectangle = new Rectangle(0, collisionFrameY, collisionTexture.Width, collisionFrameHeight);

                Main.EntitySpriteDraw(collisionTexture, beamEnd, collisionSourceRectangle, Color.White, Projectile.rotation, collisionOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);
            }


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
