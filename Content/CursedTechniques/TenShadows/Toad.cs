using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using ReLogic.Content;

namespace sorceryFight.Content.CursedTechniques.TenShadows
{
    public class Toad : CursedTechniqueSummon
    {
        public override SummonStyle Style => SummonStyle.Sentry;
        public override bool SentryTileCollide => true;

        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.Toad.DisplayName");
        public override string Description =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Toad.Description");
        public override string LockedDescription =>
            SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Toad.LockedDescription");

        public override float Cost => 40f;
        public override float CursedCostPerSecond => 2f;
        public override Color textColor => new Color(60, 140, 50);
        public override bool DisplayNameInGame => true;
        public override int Damage => 15;
        public override int MasteryDamageMultiplier => 30;
        public override float Speed => 0f;
        public override float LifeTime => 0f;
        public override float DetectionRange => 600f;
        public override string ParentInnateName => "TenShadows";

        private const int FRAME_COUNT = 5;
        private const int TICKS_PER_FRAME = 5;
        private const float SPAWN_DELAY = 60f;

        public static Texture2D texture;

        private bool ready = false;
        private int tongueIndex = -1;

        private int mouthFrame = 0;
        private int mouthFrameCounter = 0;
        private bool closing = false;
        private int lastTargetWhoAmI = -1;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<Toad>();
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = FRAME_COUNT;
        }

        public override void SummonSetDefaults()
        {
            Projectile.width = 50;
            Projectile.height = 40;
            ready = false;
            tongueIndex = -1;
            mouthFrame = 0;
            mouthFrameCounter = 0;
            closing = false;
            lastTargetWhoAmI = -1;
        }

        public override void SummonAI()
        {
            base.SummonAI();

            Projectile.velocity.X = 0f;

            SetTarget();

            if (Target != null)
                Projectile.spriteDirection = Target.Center.X > Projectile.Center.X ? 1 : -1;

            // Wait for spawn delay
            if (!ready)
            {
                if (SummonTimer >= SPAWN_DELAY)
                    ready = true;
                else
                    return;
            }

            // Detect target switch — reset mouth animation
            int currentTargetId = Target != null ? Target.whoAmI : -1;
            if (currentTargetId != lastTargetWhoAmI && currentTargetId != -1)
            {
                lastTargetWhoAmI = currentTargetId;
                mouthFrame = 0;
                mouthFrameCounter = 0;
                closing = false;

                // Kill existing tongue so it respawns after the new open animation
                if (Projectile.owner == Main.myPlayer && tongueIndex >= 0 && Main.projectile.IndexInRange(tongueIndex))
                {
                    Projectile tongue = Main.projectile[tongueIndex];
                    if (tongue.active && tongue.type == ModContent.ProjectileType<ToadTongue>())
                        tongue.Kill();
                    tongueIndex = -1;
                }
            }

            // Mouth animation
            if (Target != null && !closing)
            {
                if (mouthFrame < FRAME_COUNT - 1)
                {
                    mouthFrameCounter++;
                    if (mouthFrameCounter >= TICKS_PER_FRAME)
                    {
                        mouthFrameCounter = 0;
                        mouthFrame++;
                    }
                }
            }
            else
            {
                closing = true;
                if (mouthFrame > 0)
                {
                    mouthFrameCounter++;
                    if (mouthFrameCounter >= TICKS_PER_FRAME)
                    {
                        mouthFrameCounter = 0;
                        mouthFrame--;
                    }
                }
                else
                {
                    closing = false;
                    lastTargetWhoAmI = -1;
                }
            }

            Projectile.frame = mouthFrame;

            // Tongue management — only when mouth is fully open
            bool mouthFullyOpen = mouthFrame >= FRAME_COUNT - 1;

            if (mouthFullyOpen && Target != null && Projectile.owner == Main.myPlayer)
            {
                bool tongueAlive = tongueIndex >= 0
                    && Main.projectile.IndexInRange(tongueIndex)
                    && Main.projectile[tongueIndex].active
                    && Main.projectile[tongueIndex].type == ModContent.ProjectileType<ToadTongue>();

                if (!tongueAlive)
                {
                    float aimRotation = (Target.Center - Projectile.Center).ToRotation();

                    tongueIndex = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center + new Vector2(Projectile.spriteDirection * (Projectile.width / 3f), -Projectile.height / 4f),
                        Vector2.Zero,
                        ModContent.ProjectileType<ToadTongue>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner,
                        ai0: 0f,
                        ai1: Projectile.whoAmI
                    );

                    if (Main.projectile.IndexInRange(tongueIndex))
                    {
                        Main.projectile[tongueIndex].originalDamage = Projectile.originalDamage;
                        Main.projectile[tongueIndex].rotation = aimRotation;
                    }
                }
            }

            Lighting.AddLight(Projectile.Center, new Vector3(0.15f, 0.3f, 0.1f));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/Toad", AssetRequestMode.ImmediateLoad).Value;

            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;
            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);
            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            SpriteEffects flip = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, lightColor, Projectile.rotation, origin, Projectile.scale, flip, 0f);

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer && tongueIndex >= 0 && Main.projectile.IndexInRange(tongueIndex))
            {
                Projectile tongue = Main.projectile[tongueIndex];
                if (tongue.active && tongue.type == ModContent.ProjectileType<ToadTongue>())
                    tongue.Kill();
            }

            if (Main.dedServ)
                return;

            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(15f, 15f),
                    DustID.JungleGrass
                );
                dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                dust.scale = Main.rand.NextFloat(1f, 1.4f);
                dust.noGravity = true;
            }
        }
    }
}