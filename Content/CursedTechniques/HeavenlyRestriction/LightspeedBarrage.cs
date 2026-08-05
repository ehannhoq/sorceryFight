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

namespace sorceryFight.Content.CursedTechniques.HeavenlyRestriction
{
    public class LightspeedBarrage : CursedTechnique
    {
        public override string InternalName => "LightspeedBarrage";

        ref float lifeTimer => ref Projectile.ai[0];
        ref float ricochets => ref Projectile.ai[1];
        private List<int> enemiesHit = new List<int>();

        private const int maxRicochets = 7;
        private const float minSpeed = 30f;
        private const float maxSpeed = 80f;
        private const float minTargetDistance = 1000f;

        public override void SetDefaults()
        {
            Projectile.width = Main.player[0].width * 2;
            Projectile.height = Main.player[0].height * 2;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }


        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();


            Projectile.rotation = Projectile.velocity.ToRotation();
            VFXManager.AddVFX(new ImpactRingVFX(center: player.Center, lifetime: 60, scale: 2f, rotation: Projectile.rotation));

            sfPlayer.immune = true;
            sfPlayer.disableRegenFromProjectiles = true;

            TargetNearestNPC();
        }


        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * speed;
                var entitySource = player.GetSource_FromThis();
                int index = Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), (int)CalculateTrueDamage(sf), 0, player.whoAmI);

                LightspeedBarrage lsBarrage = Main.projectile[index].ModProjectile as LightspeedBarrage;

                if (lsBarrage.GetNearestNPCPos(out Vector2 _))
                    sf.cursedEnergy -= CalculateTrueCost(sf);

                return index;
            }
            return -1;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (ricochets >= maxRicochets)
            {
                Player player = Main.player[Projectile.owner];
                KillProjectile(player.SorceryFight());
                return;
            }

            ricochets++;
            enemiesHit.Add(target.whoAmI);
            VFXManager.AddVFX(new ImpactCircleVFX(center: target.Center, lifetime: 60, scale: 2f));
            SoundEngine.PlaySound(SorceryFightSounds.DashImpact, target.Center);
            TargetNearestNPC();

            if (Main.myPlayer == Projectile.owner)
            {
                CameraController.CameraShake(20, 15f, 5f);
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            player.velocity = Projectile.velocity;
            Projectile.Center = player.Center;
            player.direction = Projectile.velocity.X > 0 ? 1 : -1;

            if (lifeTimer > lifetime)
            {

                KillProjectile(player.SorceryFight());
            }

            lifeTimer++;
        }

        private void TargetNearestNPC()
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            float speedDiff = maxSpeed - minSpeed;
            float trueSpeed = sfPlayer.unlockedRCT ? (sfPlayer.numberBossesDefeated / SorceryFightMod.totalBosses * speedDiff) + minSpeed : (sfPlayer.numberBossesDefeated / (SorceryFightMod.totalBosses / 1.5f) * speedDiff) + minSpeed;


            if (GetNearestNPCPos(out Vector2 position))
            {
                lifeTimer = 0;
                Projectile.velocity = Projectile.Center.DirectionTo(position) * trueSpeed;
            }
            else
            {
                KillProjectile(sfPlayer);
            }
        }


        private bool GetNearestNPCPos(out Vector2 position)
        {
            float minDistance = minTargetDistance;
            NPC closestNPC = null;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.type == NPCID.TargetDummy) continue;
                if (enemiesHit.Contains(npc.whoAmI)) continue;

                float dist = (Projectile.Center - npc.Center).Length();

                if (dist < minDistance)
                {
                    if (HasLOS(Projectile.Center, npc.Center))
                    {
                        minDistance = dist;
                        closestNPC = npc;
                    }
                }
            }

            if (closestNPC == null)
            {
                position = Vector2.Zero;
                return false;
            }

            position = closestNPC.Center;
            return true;
        }


        private bool HasLOS(Vector2 start, Vector2 end)
        {
            Vector2 direction = end - start;
            direction.Normalize();

            float distance = (start - end).Length();

            for (float currentDistance = 0; currentDistance < distance; currentDistance += 0.1f)
            {
                Point tilePos = (start + direction * currentDistance).ToTileCoordinates();

                if (!WorldGen.InWorld(tilePos.X, tilePos.Y))
                    break;

                Tile tile = Main.tile[tilePos];

                bool walkableTile = !tile.HasTile || !Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] || tile.IsActuated;

                bool passable = walkableTile || (tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Water);

                if (!passable)
                    return false;
            }

            return true;
        }


        private void KillProjectile(SorceryFightPlayer sfPlayer)
        {
            Projectile.Kill();
            sfPlayer.immune = false;
            sfPlayer.disableRegenFromProjectiles = false;
            sfPlayer.Player.velocity = Vector2.Zero;

            enemiesHit = new();
            ricochets = 0;
        }
    }
}