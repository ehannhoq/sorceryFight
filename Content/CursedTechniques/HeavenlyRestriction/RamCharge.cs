using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.VFX;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;

namespace sorceryFight.Content.CursedTechniques.HeavenlyRestriction
{
    public class RamCharge : CursedTechnique
    {
        public override string InternalName => "RamCharge";

        ref float tick => ref Projectile.ai[0];
        private const float minSpeed = 10f;
        private const float maxSpeed = 20f;

        private Vector2 playerLastPos;

        public RamCharge()
        {
            Technique.baseDamage = 100;
            Technique.damagePerBoss = 6;
            Technique.cost = 5;
            Technique.speed = minSpeed;
            Technique.lifetime = 40;
        }

        public override float CalculateTrueCost(SorceryFightPlayer sf)
        {
            float speedDiff = maxSpeed - minSpeed;
            float trueSpeed = sf.unlockedRCT ? ((float)sf.numberBossesDefeated / SorceryFightMod.totalBosses * speedDiff) + minSpeed : (sf.numberBossesDefeated / (SorceryFightMod.totalBosses / 1.5f) * speedDiff) + minSpeed;

            float adjustedCost = cost * trueSpeed;
            float finalCost = adjustedCost - (adjustedCost * (sf.bossesDefeated.Count / 100f));
            finalCost *= 1 - sf.ctCostReduction;

            return finalCost;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            
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


            float speedDiff = maxSpeed - minSpeed;
            float trueSpeed = ((float)sfPlayer.numberBossesDefeated / SorceryFightMod.totalBosses * speedDiff) + minSpeed;
            float playerSpeedMultiplier = player.moveSpeed / 2.5f;
            trueSpeed *= playerSpeedMultiplier > 1 ? playerSpeedMultiplier : 1f;
            trueSpeed *= sfPlayer.unlockedRCT ? 1.5f : 1f;
            Projectile.velocity.Normalize();
            Projectile.velocity *= trueSpeed;
            playerLastPos = player.Center;
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.velocity = Projectile.velocity;
            Projectile.Center = player.Center;
            player.direction = Projectile.velocity.X > 0 ? 1 : -1;
            Projectile.velocity.Y += 0.3f;

            if (++tick % 10 == 0)
            {
                VFXManager.AddVFX(new ImpactRingVFX(center: player.Center, lifetime: 60, rotation: (playerLastPos - player.Center).ToRotation(), scale: 2f));
                playerLastPos = player.Center;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SorceryFightPlayer sfPlayer = Main.player[Projectile.owner].SorceryFight();
            sfPlayer.immune = false;
            sfPlayer.disableRegenFromProjectiles = false;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= Projectile.velocity.Length() / 16f;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            VFXManager.AddVFX(new ImpactCircleVFX(center: target.Center, lifetime: 60, scale: 2f));
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
            return false;
        }
    }
}