using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using System.Reflection.Metadata.Ecma335;
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
        private const int FALL_DAMAGE_MULTIPLIER = 5;

        private bool hasLanded = false;
        private int beamIndex = -1;
        private Vector2 lastPosition;

        public float animScale;

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
            animScale = 1.5f;

        }

        //we still want it to do some contact damage, just less than falling
        public override bool? CanDamage()
        {
            return true; 
            //!hasLanded ? null : false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!hasLanded)
            {
                modifiers.FinalDamage *= FALL_DAMAGE_MULTIPLIER;
            }
        }

        public override void SummonAI()
        {
            base.SummonAI();

            Main.NewText($"Has Elephant landed: {hasLanded} Y velocity={Projectile.velocity.Y}");

            if (!hasLanded)
            {
                // check if falling
                if (SummonTimer > 5f && MathF.Abs(Projectile.position.Y - lastPosition.Y) < 0.1f)
                {
                    hasLanded = true;
                    Projectile.velocity = Vector2.Zero;
                    Projectile.netUpdate = true;

                    // Landing impact dust
                    if (!Main.dedServ)
                    {
                        for (int i = 0; i < 30; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(
                                Projectile.Bottom + new Vector2(Main.rand.NextFloat(-40f, 40f), 0f),
                                DustID.Sand
                            );
                            dust.velocity = new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-8f, -2f));
                            dust.scale = Main.rand.NextFloat(1.2f, 2f);
                            dust.noGravity = false;
                        }
                    }
                }

                lastPosition = Projectile.position;

                AnimateFrames(FRAME_COUNT, TICKS_PER_FRAME);
                return;
            }

            Projectile.velocity = Vector2.Zero;

            SetTarget();

            if (Target != null && Projectile.owner == Main.myPlayer)
            {
                bool beamAlive = beamIndex >= 0 && Main.projectile.IndexInRange(beamIndex)
                    && Main.projectile[beamIndex].active
                    && Main.projectile[beamIndex].type == ModContent.ProjectileType<MaxElephantBeam>();

                if (!beamAlive)
                {
                    float aimRotation = (Target.Center - Projectile.Center).ToRotation();

                    beamIndex = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center + new Vector2(0f, -Projectile.height / 2f),
                        Vector2.Zero,
                        ModContent.ProjectileType<MaxElephantBeam>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner,
                        ai0: 0f,
                        ai1: Projectile.whoAmI
                    );

                    if (Main.projectile.IndexInRange(beamIndex))
                    {
                        Main.projectile[beamIndex].originalDamage = Projectile.originalDamage;
                        Main.projectile[beamIndex].rotation = aimRotation;
                    }
                }
            }

            AnimateFrames(FRAME_COUNT, TICKS_PER_FRAME);

            Main.NewText($"Target null={Target == null}, spriteDir={Projectile.spriteDirection}");

            if (Target != null)
                Projectile.spriteDirection = Projectile.Center.X < Target.Center.X ? -1 : 1;

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
                animScale,
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