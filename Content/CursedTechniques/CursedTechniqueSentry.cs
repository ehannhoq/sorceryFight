using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System;
using System.IO;

namespace sorceryFight.Content.CursedTechniques
{
    /// <summary>
    /// Base class for sentry-type cursed techniques.
    /// Calling UseTechnique toggles the sentry: if it doesn't exist, it spawns one;
    /// if one already exists for this player, it kills it (unsummon).
    /// 
    /// Subclasses should:
    ///   - Override GetProjectileType() to return the sentry projectile type
    ///   - Override GetSentryProjectileType() if the sentry body is a DIFFERENT type from the technique itself
    ///   - Override SentryAI() for the sentry body's behavior
    ///   - Override SentrySetDefaults() for sentry-specific defaults
    ///   - Set abstract properties (Damage, Cost, Speed, etc.)
    /// </summary>
    public abstract class CursedTechniqueSentry : CursedTechnique
    {
        // ── Sentry Configuration ──────────────────────────────────────

        /// <summary>
        /// The detection range for enemies, in pixels. Default 1000f (~62.5 tiles).
        /// </summary>
        public virtual float DetectionRange => 1000f;

        /// <summary>
        /// Whether the sentry should collide with tiles (ground-based) or float.
        /// </summary>
        public virtual bool SentryTileCollide => true;

        /// <summary>
        /// CE cost drained per second while the sentry is active. 0 = no drain.
        /// </summary>
        public virtual float CEDrainPerSecond => 0f;

        /// <summary>
        /// If true, the sentry hovers near the player. If false, it stays where placed.
        /// </summary>
        public virtual bool FollowsPlayer => false;

        /// <summary>
        /// How far behind/above the player the sentry hovers (only if FollowsPlayer = true).
        /// </summary>
        public virtual Vector2 FollowOffset => new Vector2(0, -60f);

        /// <summary>
        /// Override this if the sentry projectile is a separate type from this technique's type.
        /// By default returns GetProjectileType().
        /// </summary>
        public virtual int GetSentryProjectileType() => GetProjectileType();

        // ── Sentry State ──────────────────────────────────────────────

        /// <summary>
        /// Timer tracked in Projectile.ai[0]. Increments every tick while the sentry is alive.
        /// </summary>
        public ref float SentryTimer => ref Projectile.ai[0];

        /// <summary>
        /// General-purpose state value tracked in Projectile.ai[1]. 
        /// Use for attack cooldowns, phase tracking, etc.
        /// </summary>
        public ref float SentryState => ref Projectile.ai[1];

        /// <summary>
        /// General-purpose value tracked in Projectile.ai[2].
        /// </summary>
        public ref float SentryData => ref Projectile.ai[2];

        // ── Core Overrides ────────────────────────────────────────────

        public override void SetDefaults()
        {
            // Base CursedTechnique defaults
            base.SetDefaults();

            // Sentry-specific defaults (mirrors Calamity's pattern)
            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = SentryTileCollide;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;

            // Let subclasses further customize
            SentrySetDefaults();
        }

        /// <summary>
        /// Override this in subclasses to set sentry-specific defaults
        /// (width, height, frames, etc.) without needing to call base.SetDefaults().
        /// </summary>
        public virtual void SentrySetDefaults() { }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        /// <summary>
        /// The sentry body itself doesn't deal contact damage — its shots do.
        /// Override this and return null or true if you want contact damage.
        /// </summary>
        public override bool? CanDamage() => false;

        // ── AI ────────────────────────────────────────────────────────

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            SorceryFightPlayer sf = owner.SorceryFight();

            // Kill the sentry if the owner is dead or disconnected
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }

            // CE drain over time
            if (CEDrainPerSecond > 0f && Projectile.owner == Main.myPlayer)
            {
                float drainPerTick = CEDrainPerSecond / 60f;
                sf.cursedEnergy -= drainPerTick;

                if (sf.cursedEnergy <= 0f)
                {
                    sf.cursedEnergy = 0f;
                    Projectile.Kill();
                    return;
                }
            }

            if (FollowsPlayer)
            {
                Vector2 destination = owner.Center + FollowOffset;
                float bobOffset = (float)Math.Sin(Projectile.timeLeft / 30f) * 4f;
                destination.Y += bobOffset;

                Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.08f);

                // teleport to player
                if (Projectile.WithinRange(destination, 4f) || !Projectile.WithinRange(destination, 2000f))
                    Projectile.Center = destination;

                Projectile.velocity = Vector2.Zero;
            }

            if (SentryTileCollide && !FollowsPlayer)
            {
                Projectile.velocity.Y += 0.5f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;
            }

            SentryTimer++;

            // use child AI
            SentryAI(owner, sf);
        }

        /// <summary>
        /// Override this with your sentry's custom behavior:
        /// targeting enemies, firing projectiles, animations, etc.
        /// Called every tick after the base sentry logic runs.
        /// </summary>
        public virtual void SentryAI(Player owner, SorceryFightPlayer sf) { }

        // ── Tile Collision (Ground Sentries) ──────────────────────────

        /// <summary>
        /// Prevents the sentry from being destroyed on tile collision.
        /// It just stops moving horizontally. Standard Calamity pattern.
        /// </summary>
        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }

        // ── Toggle Logic (UseTechnique) ───────────────────────────────

        /// <summary>
        /// Overrides the base UseTechnique to toggle the sentry on/off.
        /// If a sentry of this type already exists for the player, kill it.
        /// Otherwise, spawn a new one at the cursor position.
        /// </summary>
        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI != Main.myPlayer)
                return -1;

            int sentryType = GetSentryProjectileType();

            // Check if a sentry of this type already exists for this player
            bool existingSentryFound = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == sentryType && proj.sentry)
                {
                    // Kill the existing sentry (unsummon)
                    proj.Kill();
                    existingSentryFound = true;
                }
            }

            if (existingSentryFound)
            {
                // Display unsummon text
                if (DisplayNameInGame)
                {
                    int index = CombatText.NewText(player.getRect(), textColor, $"{DisplayName.Value} - Dismissed");
                    Main.combatText[index].lifeTime = 120;
                }
                return -1;
            }

            // ── Spawn new sentry ──

            // Deduct costs
            sf.cursedEnergy -= CalculateTrueCost(sf);

            if (BloodCost > 0)
                sf.bloodEnergy -= BloodCost;

            if (StarCost > 0)
                sf.starEnergy -= StarCost;

            // Display summon text
            if (DisplayNameInGame)
            {
                int index = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                Main.combatText[index].lifeTime = 180;
            }

            // Spawn the sentry at the cursor (like Calamity's CryogenicStaff)
            Vector2 spawnPos = Main.MouseWorld;
            var source = player.GetSource_FromThis();

            int projIndex = Projectile.NewProjectile(
                source,
                spawnPos,
                Vector2.Zero,
                sentryType,
                (int)CalculateTrueDamage(sf),
                0f,
                player.whoAmI
            );

            if (Main.projectile.IndexInRange(projIndex))
            {
                Main.projectile[projIndex].originalDamage = Damage;
            }

            player.UpdateMaxTurrets();

            return projIndex;
        }

        // ── Network Sync ──────────────────────────────────────────────

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.ai[0]);
            writer.Write(Projectile.ai[1]);
            writer.Write(Projectile.ai[2]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.ai[0] = reader.ReadSingle();
            Projectile.ai[1] = reader.ReadSingle();
            Projectile.ai[2] = reader.ReadSingle();
        }

        // ── Helpers ───────────────────────────────────────────────────

        /// <summary>
        /// Finds the closest NPC within detection range that can be targeted.
        /// Respects the player's minion attack target (whip targeting).
        /// Similar to Calamity's IceSentry/Hive targeting pattern.
        /// </summary>
        protected NPC FindTarget(Player owner)
        {
            // Priority: player's manually-selected target
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC manualTarget = Main.npc[owner.MinionAttackTargetNPC];
                if (manualTarget.CanBeChasedBy(Projectile) &&
                    Vector2.Distance(Projectile.Center, manualTarget.Center) < DetectionRange)
                {
                    return manualTarget;
                }
            }

            // Fallback: closest valid NPC
            NPC bestTarget = null;
            float bestDist = DetectionRange;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile))
                    continue;

                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestTarget = npc;
                }
            }

            return bestTarget;
        }

        /// <summary>
        /// Counts how many sentries of a given type this player currently owns.
        /// </summary>
        protected int CountOwnedSentries(int projectileType)
        {
            int count = 0;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == Projectile.owner && proj.type == projectileType)
                    count++;
            }
            return count;
        }

        /// <summary>
        /// Basic frame animation helper. Call from SentryAI.
        /// </summary>
        protected void AnimateFrames(int totalFrames, int ticksPerFrame)
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= ticksPerFrame)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = (Projectile.frame + 1) % totalFrames;
            }
        }
    }
}
