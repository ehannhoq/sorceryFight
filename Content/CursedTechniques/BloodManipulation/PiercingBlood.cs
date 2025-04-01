using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class PiercingBlood : CursedTechnique
    {
        public static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.PiercingBlood.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PiercingBlood.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PiercingBlood.LockedDescription");
        public override float Cost => 750f;
        public override Color textColor => new Color(132, 4, 4);
        public override bool DisplayNameInGame => true;
        public override int Damage => 100;
        public override int MasteryDamageMultiplier => 18;
        public override float Speed => 0f;
        public override float LifeTime => 100f;

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
                Main.player[Projectile.owner].GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
            }

            if (beamHeight < 1.0f && Projectile.timeLeft > 10)
                beamHeight += 0.1f;

            if (Projectile.timeLeft <= 10)
            {
                beamHeight -= 0.1f;
                Main.player[Projectile.owner].GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                Player player = Main.player[Projectile.owner];

                Projectile.Center = player.Center;

                float targetRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.rotation = SFUtils.LerpAngle(Projectile.rotation, targetRotation, 0.2f);
                Projectile.direction = Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1;
                player.ChangeDir(Projectile.direction);

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

            Vector2 particleVelocityOffset = new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.2f, 0.2f));
            Vector2 particlePositionOffset = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));
            Vector2 particleVelocity = (Projectile.rotation.ToRotationVector2() + particleVelocityOffset).SafeNormalize(Vector2.UnitX) * 10f;
            LineParticle particle = new LineParticle(Projectile.Center + particlePositionOffset, particleVelocity, false, 30, 1.25f, textColor);
            GeneralParticleHandler.SpawnParticle(particle);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float beamLength = Projectile.localAI[0];
            Vector2 start = Projectile.Center - Main.screenPosition;
            Vector2 scale = new Vector2(beamLength / texture.Width, BASE_BEAM_HEIGHT * beamHeight);
            Vector2 origin = new Vector2(0, texture.Height / 2);

            Main.EntitySpriteDraw(texture, start, null, Color.White, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);
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
