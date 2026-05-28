using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ReLogic.Content;


namespace sorceryFight.Content.CursedTechniques.TenShadows
{
    //child of white
    public class DivineBlack : CursedTechniqueSummon
    {
        public override SummonStyle Style => SummonStyle.GroundedMinion;

        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.DivineBlack.DisplayName");
        public override string Description =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineBlack.Description");
        public override string LockedDescription =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineBlack.LockedDescription");

        public override float Cost => 0f;
        public override Color textColor => new Color(30, 30, 40);
        public override bool DisplayNameInGame => false;
        public override int Damage => 25;
        public override int MasteryDamageMultiplier => 40;
        public override float Speed => 8f;
        public override float LifeTime => 0f;
        public override float DetectionRange => 800f;
        public override float Gravity => 0.4f;
        public override float MaxFallSpeed => 12f;
        public override bool CanContactDamage => true;

        public override Color selectorBGColor { get; set; }
        public override Color selectorBorderColor { get; set; }

        private const int FRAME_COUNT = 7;
        private const int TICKS_PER_FRAME = 6;

        private bool IsMoving => MathF.Abs(Projectile.velocity.X) > 0.5f || MathF.Abs(Projectile.velocity.Y) > 0.5f;

        public static Texture2D texture;
        public static Texture2D runTexture;

        public override string ParentInnateName => "TenShadows";

        public override bool Unlocked(SorceryFightPlayer sf) => true;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<DivineBlack>();
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            //should never get here
            return -1;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = FRAME_COUNT;
        }

        public override void SummonSetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
        }
  
        public override void SummonAI()
        {
            base.SummonAI();

            AnimateFrames(FRAME_COUNT, TICKS_PER_FRAME);

            if (Target != null)
            {
                HopToward(Target.Center);
                Projectile.spriteDirection = MathF.Sign(Target.Center.X - Projectile.Center.X);
            }
            else
            {
                if (!Projectile.WithinRange(Owner.Center, 150f))
                    HopToward(Owner.Center);

                if (MathF.Abs(Projectile.velocity.X) > 0.5f)
                    Projectile.spriteDirection = MathF.Sign(Projectile.velocity.X);
            }

            if (!Projectile.WithinRange(Owner.Center, 2000f))
            {
                Projectile.Center = Owner.Center;
                Projectile.netUpdate = true;
            }
        }

        private bool OnGround => MathF.Abs(Projectile.velocity.Y) < 0.1f;

        private void HopToward(Vector2 target)
        {
            if (OnGround && SummonTimer % 30f >= 29f)
            {
                Vector2 dir = (target - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity = dir * Speed + new Vector2(MathF.Sign(dir.X) * 2f, -8f);
                Projectile.tileCollide = false;
                Projectile.netUpdate = true;
            }
            else
            {
                Projectile.tileCollide = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects flip = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (IsMoving)
            {
                if (runTexture == null && !Main.dedServ)
                    runTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/DivineBlackRun", AssetRequestMode.ImmediateLoad).Value;

                int frameHeight = runTexture.Height / FRAME_COUNT;
                int frameY = Projectile.frame * frameHeight;
                Vector2 origin = new Vector2(runTexture.Width / 2, frameHeight / 2);
                Rectangle sourceRectangle = new Rectangle(0, frameY, runTexture.Width, frameHeight);

                spriteBatch.Draw(runTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }
            else
            {
                if (texture == null && !Main.dedServ)
                    texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/DivineBlack", AssetRequestMode.ImmediateLoad).Value;

                Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }
            return false;
        }

    }
}
