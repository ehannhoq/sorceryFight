using System;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Steamworks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.HeavenlyRestriction
{
    public class RamCharge : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.RamCharge.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.RamCharge.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.RamCharge.LockedDescription");

        public override float Cost => 30f;

        public override Color textColor => Color.White;

        public override bool DisplayNameInGame => false;

        public override int Damage => 60;

        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 15f;

        public override float LifeTime => 40;

        private static Texture2D impactTexture;
        ref float tick => ref Projectile.ai[0];
        private Vector2 startPos;
        private Vector2 startVel;

        private const float minSpeed = 20f;
        private const float maxSpeed = 60f;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<RamCharge>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }

        public override void SetStaticDefaults()
        {
            impactTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/HeavenlyRestriction/ImpactRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = Main.player[0].width * 2;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            sfPlayer.immune = true;
            sfPlayer.disableRegenFromProjectiles = true;

            startPos = player.Center;
            startVel = Projectile.velocity;

            float speedDiff = maxSpeed - minSpeed;
            float trueSpeed = sfPlayer.leftItAllBehind ? (sfPlayer.numberBossesDefeated / SorceryFight.totalBosses * speedDiff) + minSpeed : (sfPlayer.numberBossesDefeated / (SorceryFight.totalBosses / 1.5f) * speedDiff) + minSpeed;
            Projectile.velocity.Normalize();
            Projectile.velocity *= trueSpeed;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.velocity = Projectile.velocity;
            Projectile.Center = player.Center;
            player.direction = Projectile.velocity.X > 0 ? 1 : -1;

            Projectile.velocity.Y += 0.3f;

            if (tick++ >= LifeTime)
            {
                Projectile.Kill();
                player.SorceryFight().immune = false;
                player.SorceryFight().disableRegenFromProjectiles = false;

            }

            if (startPos != Vector2.Zero)
            {
                startPos -= Projectile.velocity * 0.1f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle impactSrc = new Rectangle(0, 0, impactTexture.Width, impactTexture.Height);


            float impactScale = MathF.Sqrt(1 - MathF.Pow((tick / 60) - 1, 2)) * Projectile.velocity.Length() / 5f;
            ;
            float impactOpacity = MathF.Sqrt(1 - MathF.Pow(tick / 60, 2));

            impactOpacity = Math.Clamp(impactOpacity, 0f, 1f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            Main.EntitySpriteDraw(impactTexture, startPos - Main.screenPosition, impactSrc, Color.White * impactOpacity, startVel.ToRotation(), impactSrc.Size() * 0.5f, impactScale, SpriteEffects.None);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

            return false;
        }
    }
}