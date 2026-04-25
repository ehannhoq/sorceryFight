using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.TenShadows
{
    public class Nue : CursedTechniqueSummon
    {
        // ── Style ─────────────────────────────────────────────────────
        public override SummonStyle Style => SummonStyle.FlyingMinion;

        // ── Technique Identity ────────────────────────────────────────
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.CursedSpiritMinion.DisplayName");
        public override string Description =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.CursedSpiritMinion.Description");
        public override string LockedDescription =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.CursedSpiritMinion.LockedDescription");

        // ── Stats ─────────────────────────────────────────────────────
        public override float Cost => 40f;
        public override float CursedCostPerSecond => 2f;
        public override Color textColor => new Color(80, 180, 255);
        public override bool DisplayNameInGame => true;
        public override int Damage => 18;
        public override int MasteryDamageMultiplier => 35;
        public override float Speed => 10f;
        public override float LifeTime => 0f;
        public override float DetectionRange => 900f;
        public override string ParentInnateName => "TenShadows";

        private const int FRAME_COUNT = 8;
        private const int TICKS_PER_FRAME = 5;
        private const float FIRE_RATE = 60f;

        public static Texture2D texture;
        public static Texture2D flyTexture;

        //private bool IsMoving => Projectile.velocity.Length() > 0.5f;
        private bool IsMoving => Target != null;
        //variable name is misleading for now, checks if there is a target


        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<Nue>();
        }

        // ── Stats Display ─────────────────────────────────────────────
        public override string GetStats(SorceryFightPlayer sf)
        {
            string stats = base.GetStats(sf);
            stats += "Toggle: Press to summon/dismiss\n";
            return stats;
        }

        // ── Setup ─────────────────────────────────────────────────────
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = FRAME_COUNT;

            if (!Main.dedServ)
            {
                texture = ModContent.Request<Texture2D>(
                    "sorceryFight/Content/CursedTechniques/TenShadows/Nue",
                    AssetRequestMode.ImmediateLoad).Value;

                flyTexture = ModContent.Request<Texture2D>(
                    "sorceryFight/Content/CursedTechniques/TenShadows/NueFly",
                    AssetRequestMode.ImmediateLoad).Value;
            }

        }

        public override void SummonSetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 36;
        }

        // ── AI ────────────────────────────────────────────────────────
        public override void SummonAI()
        {
            base.SummonAI();

            SorceryFightMod.Log.Info($"Nue AI ticking on netMode={Main.netMode} owner={Projectile.owner} myPlayer={Main.myPlayer}");

            bool returning = ShouldReturnToOwner();

            if (Target != null && !returning)
            {
                FlyTowardTarget(approachSpeed: 14f, retreatSpeed: 6f, preferredDist: 180f);
                AttackTarget();
            }
            else
            {
                FlyNearOwner(hoverDistance: 140f, returnSpeed: 15f);
                Projectile.velocity.Y += MathF.Sin(Projectile.timeLeft * 0.05f) * 0.03f;
                SummonState = FIRE_RATE * 0.7f;
            }

            AntiClump();
            AnimateFrames(FRAME_COUNT, TICKS_PER_FRAME);

            if (Target != null && !returning)
                Projectile.spriteDirection = MathF.Sign(Target.Center.X - Projectile.Center.X);
            else if (MathF.Abs(Projectile.velocity.X) > 0.5f)
                Projectile.spriteDirection = MathF.Sign(Projectile.velocity.X);

            if (Projectile.owner == Main.myPlayer)
                Lighting.AddLight(Projectile.Center, new Vector3(2f, 0f, 0f)); // bright red = owner client
            else
                Lighting.AddLight(Projectile.Center, new Vector3(0f, 2f, 0f)); // bright green = other client
        }

        private void AttackTarget()
        {
            SummonState++;
            if (SummonState >= FIRE_RATE && Projectile.owner == Main.myPlayer)
            {
                SummonState = 0f;
                Projectile.netUpdate = true;

                Vector2 direction = (Target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
                Vector2 velocity = direction * Speed;

                int bolt = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<NueLightning>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );

                if (Main.projectile.IndexInRange(bolt))
                    Main.projectile[bolt].originalDamage = Projectile.originalDamage;

                SoundEngine.PlaySound(SoundID.Item8 with { Volume = 0.7f, Pitch = 0.3f }, Projectile.Center);
            }
        }

        // ── Drawing ───────────────────────────────────────────────────
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects flip = Projectile.spriteDirection == -1
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;

            SorceryFightMod.Log.Info($"frame={Projectile.frame} spriteDirection={Projectile.spriteDirection} IsMoving={IsMoving} Target={Target?.TypeName ?? "null"} owner={Projectile.owner} myPlayer={Main.myPlayer}");
            
            if (IsMoving)
            {
                Vector2 origin = new Vector2(flyTexture.Width / 2, flyTexture.Height / 2);
                spriteBatch.Draw(flyTexture, Projectile.Center - Main.screenPosition,
                    null, lightColor, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }
            else
            {
                int frameHeight = texture.Height / FRAME_COUNT;
                Rectangle sourceRectangle = new Rectangle(0, Projectile.frame * frameHeight,
                    texture.Width, frameHeight);
                Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition,
                    sourceRectangle, lightColor, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }

            return false;
        }

        // ── Death FX ──────────────────────────────────────────────────
        public override void OnKill(int timeLeft)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 15; i++)
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                        DustID.BlueTorch
                    );
                    dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                    dust.scale = Main.rand.NextFloat(1f, 1.4f);
                    dust.noGravity = true;
                }
            }

            base.OnKill(timeLeft);
        }
    }

}
