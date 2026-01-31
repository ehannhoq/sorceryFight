using System;
using System.Collections.Generic;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.SFPlayer;
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
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.LightspeedBarrage.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.LightspeedBarrage.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.LightspeedBarrage.LockedDescription");

        public override float Cost => 670f;

        public override Color textColor => Color.White;

        public override bool DisplayNameInGame => false;

        public override int Damage => 30000;

        public override int MasteryDamageMultiplier => 300;

        public override float Speed => 15f;

        public override float LifeTime => 30;

        private static Texture2D impactCircleTexture;
        private static Texture2D impactRingTexture;

        ref float lifeTimer => ref Projectile.ai[0];

        ref float ricochets => ref Projectile.ai[1];
        private List<int> enemiesHit = new List<int>();
        private Vector2 startPos;

        private Dictionary<Vector2, int> ricochetPositions = new();


        private const int maxRicochets = 7;
        private const float minSpeed = 30f;
        private const float maxSpeed = 80f;
        private const float minTargetDistance = 1000f;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<LightspeedBarrage>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<Signus>()) && sf.HasDefeatedBoss(ModContent.NPCType<StormWeaverHead>()) && sf.HasDefeatedBoss(ModContent.NPCType<CeaselessVoid>());
        }

        public override void SetStaticDefaults()
        {
            impactCircleTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/HeavenlyRestriction/ImpactCircle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            impactRingTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/HeavenlyRestriction/ImpactRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

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

            startPos = player.Center;
            ricochetPositions.Add(startPos, 0);
            Projectile.rotation = Projectile.velocity.ToRotation();

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
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * Speed;
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
            ricochetPositions.Add(target.Center, 0);
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

            if (lifeTimer > LifeTime)
            {

                KillProjectile(player.SorceryFight());
            }

            lifeTimer++;

            foreach (var kvp in ricochetPositions)
            {
                var key = kvp.Key;
                ricochetPositions[key]++;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            foreach (var kvp in ricochetPositions)
            {
                var tick = kvp.Value;
                var position = kvp.Key;
                float t = Math.Clamp(tick / 60f, 0f, 1f);

                float impactOpacity = MathF.Sqrt(1f - t * t);

                float scaleT = Math.Clamp(1f - MathF.Pow(t - 1f, 2f), 0f, 1f);
                float impactScale = MathF.Sqrt(scaleT) * 2.5f;

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

                if (position == startPos)
                {
                    Rectangle impactRingSrc = new Rectangle(0, 0, impactRingTexture.Width, impactRingTexture.Height);
                    Main.EntitySpriteDraw(impactRingTexture, position - Main.screenPosition, impactRingSrc, Color.White * impactOpacity, Projectile.rotation, impactRingSrc.Size() * 0.5f, impactScale, SpriteEffects.None);
                    continue;
                }

                Rectangle impactCircleSrc = new Rectangle(0, 0, impactCircleTexture.Width, impactCircleTexture.Height);
                Main.EntitySpriteDraw(impactCircleTexture, position - Main.screenPosition, impactCircleSrc, Color.White * impactOpacity, 0f, impactCircleSrc.Size() * 0.5f, impactScale, SpriteEffects.None);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
            }

            return false;
        }

        private void TargetNearestNPC()
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            float speedDiff = maxSpeed - minSpeed;
            float trueSpeed = sfPlayer.leftItAllBehind ? (sfPlayer.numberBossesDefeated / SorceryFight.totalBosses * speedDiff) + minSpeed : (sfPlayer.numberBossesDefeated / (SorceryFight.totalBosses / 1.5f) * speedDiff) + minSpeed;


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
                if (npc.friendly || npc.type == NPCID.TargetDummy || npc.type == ModContent.NPCType<SuperDummyNPC>()) continue;
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
            ricochetPositions = new();
            ricochets = 0;
        }
    }
}