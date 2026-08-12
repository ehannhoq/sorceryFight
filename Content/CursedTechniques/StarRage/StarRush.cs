using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.VFX;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class StarRush : CursedTechnique
    {
        public override string InternalName => "StarRush";

        ref float tick => ref Projectile.ai[0];

        private const float minSpeed = 10f;
        private const float maxSpeed = 20f;

        // public override float Cost => 5f;
        // public override int Damage => 150;
        // public override int MasteryDamageMultiplier => 175;
        // public override float Speed => 15f;
        // public override float LifeTime => 40;


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

            VFXManager.AddVFX(new ImpactRingVFX(center: player.Center, lifetime: 60, rotation: Projectile.velocity.ToRotation(), scale: 2f));

            float speedDiff = maxSpeed - minSpeed;
            float trueSpeed = ((float)sfPlayer.numberBossesDefeated / SorceryFightMod.totalBosses * speedDiff) + minSpeed;
            float playerSpeedMultiplier = player.moveSpeed / 2.5f;
            trueSpeed *= playerSpeedMultiplier > 1 ? playerSpeedMultiplier : 1f;
            trueSpeed *= sfPlayer.unlockedRCT ? 1.5f : 1f;
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

            if (tick++ >= lifetime)
            {
                Projectile.Kill();

                player.SorceryFight().immune = false;
                player.SorceryFight().disableRegenFromProjectiles = false;
            }

            // Starry trail
            if (Projectile.velocity != Vector2.Zero)
            {
                int dustType = Main.rand.NextBool() ? DustID.BlueFairy : DustID.PinkFairy;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnOffset = new Vector2(
                        Main.rand.NextFloat(-player.width * 0.4f, player.width * 0.4f),
                        Main.rand.NextFloat(-player.height * 0.4f, player.height * 0.4f)
                    );

                    Dust star = Dust.NewDustDirect(
                        player.Center + spawnOffset - new Vector2(4f),
                        0, 0,
                        dustType,
                        -Projectile.velocity.X * 0.15f,
                        -Projectile.velocity.Y * 0.15f
                    );

                    star.scale = Main.rand.NextFloat(0.6f, 1.4f);
                    star.fadeIn = Main.rand.NextFloat(0.4f, 0.9f);
                    star.noGravity = true;
                    star.color = Main.rand.Next(3) switch
                    {
                        0 => new Color(160, 80, 255),   // vivid purple
                        1 => new Color(80, 0, 120),     // deep dark purple
                        _ => new Color(200, 150, 255)   // soft lavender
                    };
                }

                // Occasional larger glowing star burst
                if (Main.rand.NextBool(6))
                {
                    Dust burst = Dust.NewDustDirect(
                        player.Center - new Vector2(4f),
                        0, 0,
                        dustType,
                        Main.rand.NextFloat(-1.5f, 1.5f),
                        Main.rand.NextFloat(-1.5f, 1.5f)
                    );
                    burst.scale = Main.rand.NextFloat(1.4f, 2.2f);
                    burst.noGravity = true;
                    burst.color = Main.rand.Next(2) switch
                    {
                        0 => new Color(140, 0, 200),    // deep violet
                        _ => new Color(10, 0, 30)       // near-black space
                    };
                }
            }
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= Projectile.velocity.Length() / 16f;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            VFXManager.AddVFX(new ImpactCircleVFX(target.Center, lifetime: 60, scale: 2f));
            SoundEngine.PlaySound(SorceryFightSounds.DashImpact, target.Center);

            //make the player stop when hitting a target
            Player player = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Zero;
            player.velocity = Vector2.Zero;
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