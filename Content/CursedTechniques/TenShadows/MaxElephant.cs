using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.TenShadows
{
    public class MaxElephant : CursedTechniqueSummon
    {
        public override SummonStyle Style => SummonStyle.Sentry;
        public override bool SentryTileCollide => true;

        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.MaxElephant.DisplayName");
        public override string Description =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.MaxElephant.Description");
        public override string LockedDescription =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.MaxElephant.LockedDescription");

        public override float Cost => 60f;
        public override float CursedCostPerSecond => 3f;
        public override Color textColor => new Color(200, 180, 100);
        public override bool DisplayNameInGame => true;
        public override int Damage => 25;
        public override int MasteryDamageMultiplier => 40;
        public override float Speed => 0f;
        public override float LifeTime => 0f;
        public override float DetectionRange => 800f;
        public override string ParentInnateName => "TenShadows";

        private const int FRAME_COUNT = 7;
        private const int TICKS_PER_FRAME = 5;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<MaxElephant>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = FRAME_COUNT;
        }

        public override void SummonSetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
        }

        public override void SummonAI()
        {
            base.SummonAI();

            // First tick — ensure velocity is never zero
            if (SummonTimer == 1f)
            {
                Projectile.velocity.X = Main.rand.NextBool() ? 1f : -1f;
                Projectile.netUpdate = true;
            }

            // Random direction change every 120 ticks
            if (Projectile.owner == Main.myPlayer && SummonTimer % 120f == 0f)
            {
                Projectile.velocity.X = Main.rand.NextBool() ? 1f : -1f;
                Projectile.netUpdate = true;
            }

            AnimateFrames(FRAME_COUNT, TICKS_PER_FRAME);

            NPC target = FindClosestNPC(DetectionRange);

            if (target != null)
                Projectile.spriteDirection = target.Center.X < Projectile.Center.X ? -1 : 1;
            else
                Projectile.spriteDirection = Projectile.velocity.X < 0f ? -1 : 1;

            Lighting.AddLight(Projectile.Center, new Vector3(0.4f, 0.35f, 0.2f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(
                "sorceryFight/Content/CursedTechniques/TenShadows/MaxElephant"
            ).Value;

            int frameHeight = tex.Height / FRAME_COUNT;
            Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * frameHeight, tex.Width, frameHeight);
            Vector2 origin = new Vector2(tex.Width / 2f, frameHeight / 2f);
            SpriteEffects flip = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(
                tex,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                flip,
                0f
            );

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(20f, 20f),
                    DustID.Sand
                );
                dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
                dust.scale = Main.rand.NextFloat(1f, 1.6f);
                dust.noGravity = false;
            }
        }
    }
}