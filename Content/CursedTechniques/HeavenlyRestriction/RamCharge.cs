using System;
using System.Collections.Generic;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.SFPlayer;
using Steamworks;
using Terraria;
using Terraria.Audio;
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

        public override float Cost => 5f;

        public override Color textColor => Color.White;

        public override bool DisplayNameInGame => false;

        public override int Damage => 150;

        public override int MasteryDamageMultiplier => 175;

        public override float Speed => 15f;

        public override float LifeTime => 40;

        private static Texture2D impactRing;
        private static Texture2D impactCircle;
        ref float tick => ref Projectile.ai[0];
        private Vector2 startPos;
        private Vector2 startVel;
        private Dictionary<Vector2, int> impactPositions = new();


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

        public override float CalculateTrueCost(SorceryFightPlayer sf)
        {
            float speedDiff = maxSpeed - minSpeed;
            float trueSpeed = sf.leftItAllBehind ? ((float)sf.numberBossesDefeated / SorceryFight.totalBosses * speedDiff) + minSpeed : (sf.numberBossesDefeated / (SorceryFight.totalBosses / 1.5f) * speedDiff) + minSpeed;

            float adjustedCost = Cost * trueSpeed;
            float finalCost = adjustedCost - (adjustedCost * (sf.bossesDefeated.Count / 100f));
            finalCost *= 1 - sf.ctCostReduction;

            return finalCost;
        }

        public override void SetStaticDefaults()
        {
            impactRing = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/HeavenlyRestriction/ImpactRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            impactCircle = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/HeavenlyRestriction/ImpactCircle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
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
            Projectile.penetrate = -1;
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
            float playerSpeedMultiplier = player.moveSpeed / 2.5f;
            trueSpeed *= playerSpeedMultiplier > 1 ? playerSpeedMultiplier : 1f;
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


            foreach (var kvp in impactPositions)
            {
                var key = kvp.Key;
                impactPositions[key]++;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= Projectile.velocity.Length() / 16f;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            impactPositions.Add(target.Center, 0);
            SoundEngine.PlaySound(SorceryFightSounds.DashImpact, target.Center);

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

            Rectangle impactSrc = new Rectangle(0, 0, impactRing.Width, impactRing.Height);

            float impactScale = MathF.Sqrt(1 - MathF.Pow((tick / 60) - 1, 2)) * Projectile.velocity.Length() / 5f;
            float impactOpacity = MathF.Sqrt(1 - MathF.Pow(tick / 60, 2));

            impactOpacity = Math.Clamp(impactOpacity, 0f, 1f);

            Main.EntitySpriteDraw(impactRing, startPos - Main.screenPosition, impactSrc, Color.White * impactOpacity, startVel.ToRotation(), impactSrc.Size() * 0.5f, impactScale, SpriteEffects.None);

            foreach (var kvp in impactPositions)
            {
                var tick = kvp.Value;
                var position = kvp.Key;

                Rectangle impactCircleSrc = new Rectangle(0, 0, impactCircle.Width, impactCircle.Height);

                float t = Math.Clamp(tick / 30f, 0f, 1f);

                float impactCircleOpacity = MathF.Sqrt(1f - t * t);

                float scaleT = Math.Clamp(1f - MathF.Pow(t - 1f, 2f), 0f, 1f);
                float impactCircleScale = MathF.Sqrt(scaleT) * 2.5f;

                Main.EntitySpriteDraw(impactCircle, position - Main.screenPosition, impactCircleSrc, Color.White * impactCircleOpacity, 0f, impactCircleSrc.Size() * 0.5f, impactCircleScale, SpriteEffects.None);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();

            return false;
        }
    }
}