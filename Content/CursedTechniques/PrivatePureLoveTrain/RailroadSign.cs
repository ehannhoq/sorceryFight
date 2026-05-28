using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using sorceryFight.SFPlayer;
using System;


namespace sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain
{
    public class RailroadSign : CursedTechnique
    {
        public static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/RailroadSign", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.RailroadSign.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.RailroadSign.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.RailroadSign.LockedDescription");
        public override float Cost => 120f;
        public override Color textColor => new Color(108, 158, 240);
        public override bool DisplayNameInGame => true;
        public override int Damage => 15000;
        public override int MasteryDamageMultiplier => 700;
        public override float Speed => 0;
        public override float LifeTime => 60f;

        public Player Owner => Main.player[Projectile.owner];

        public int Direction => Math.Sign(Main.MouseWorld.X - Owner.Center.X);
       
        public int SwingTime = 60;

        public float SwingCompletion => MathHelper.Clamp(Time / SwingTime, 0f, 1f);

        public float SwingCompletionAtStartOfTrail
        {
            get
            {
                float swingCompletion = SwingCompletion - 0.2f;
                return MathHelper.Clamp(swingCompletion, SwingCompletionRatio, 1f);
            }
        }

        public float SwordRotation
        {
            get
            {
                // float swordRotation = InitialRotation + GetSwingOffsetAngle(SwingCompletion) * Projectile.spriteDirection + MathHelper.PiOver4;
                float swordRotation = InitialRotation * Projectile.spriteDirection + MathHelper.PiOver4;
                if (Projectile.spriteDirection == -1)
                    swordRotation += MathHelper.PiOver2;
                return swordRotation;
            }
        }

        public Vector2 SwordDirection => SwordRotation.ToRotationVector2() * Direction;

        public ref float Time => ref Projectile.ai[0];

        public ref float InitialRotation => ref Projectile.ai[1];

        public static float SwingCompletionRatio => 0.37f;

        public static float RecoveryCompletionRatio => 0.84f;
        public float SlashWidthFunction(float completionRatio) => Projectile.scale * 22f;

        // public static CurveSegment AnticipationWait => new(EasingType.PolyOut, 0f, -1.67f, 0f);
        // public static CurveSegment Anticipation => new(EasingType.PolyOut, 0.14f, AnticipationWait.EndingHeight, -1.1f, 3);
        // public static CurveSegment Swing => new(EasingType.PolyIn, SwingCompletionRatio, Anticipation.EndingHeight, 6.5f, 5);
        // public static CurveSegment Recovery => new(EasingType.PolyOut, RecoveryCompletionRatio, Swing.EndingHeight, 0.97f, 3);

        // public static float GetSwingOffsetAngle(float completion) => PiecewiseAnimation(completion, AnticipationWait, Anticipation, Swing, Recovery);
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return true;
        }
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<RailroadSign>();
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = (int)LifeTime;
            Projectile.scale = 1.75f;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            return base.UseTechnique(sf);
        }

        public override void OnSpawn(IEntitySource source)
        {

            for (int i = 0; i < Main.projectile.Length; i++)
            {
                if (i == Projectile.whoAmI)
                    continue;

                Projectile proj = Main.projectile[i];

                if (proj.type == ModContent.ProjectileType<RailroadSign>() && proj.owner == Projectile.owner)
                {
                    proj.active = false;
                }

            }
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.SorceryFight().disableRegenFromProjectiles = true;
            if (InitialRotation == 0f)
            {
                InitialRotation = Projectile.velocity.ToRotation();
                Projectile.netUpdate = true;
            }
            if(Projectile.timeLeft == 30)
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffSwing, player.Center);

            Projectile.scale = Utils.GetLerpValue(0f, 0.13f, SwingCompletion, true) * Utils.GetLerpValue(1f, 0.87f, SwingCompletion, true) * 0.7f + 0.3f;

            AdjustPlayerValues();
            StickToOwner();
            Projectile.rotation = SwordRotation;
            Time++;

            
        }

        public void AdjustPlayerValues()
        {
            Projectile.spriteDirection = Projectile.direction = Direction;
            float armRotation = SwordRotation - Direction * 1.67f;
            Owner.SetCompositeArmFront(Math.Abs(armRotation) > 0.01f, Player.CompositeArmStretchAmount.Full, armRotation);
        }

        public void StickToOwner()
        {
           Vector2 holdOffset = new Vector2(0f, texture.Height * 0.1f) * Projectile.scale;
            Projectile.Center = Owner.RotatedRelativePoint(Owner.MountedCenter, true)
                - SwordDirection * holdOffset;
            Owner.ChangeDir(Direction);
        }

        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * Vector2.UnitY;
            if (Projectile.spriteDirection == -1)
                origin.X += texture.Width;

            SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.spriteBatch.Draw(texture, drawPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, direction, 0f);
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float rotation = SwordRotation;
            if (Projectile.spriteDirection == -1)
                rotation += MathHelper.Pi;
            Vector2 direction = rotation.ToRotationVector2();

            Vector2 start = Projectile.Center;

            Vector2 end = start + direction * texture.Height * Projectile.scale * 0.9f;

            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end,Projectile.width * 0.25f, ref collisionPoint) && Projectile.timeLeft <= 35;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.velocity = Vector2.Zero;
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.SorceryFight().disableRegenFromProjectiles = false;
        }

    }
}