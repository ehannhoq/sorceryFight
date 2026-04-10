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
    /// Defines how the summon behaves.
    /// </summary>
    public enum SummonStyle
    {
        /// <summary>Stays where placed. Gravity optional. Does not follow the player.</summary>
        Sentry,

        /// <summary>Flies freely, hovers near the player, chases enemies.</summary>
        FlyingMinion,

        /// <summary>Walks on the ground, hops toward the player and enemies.</summary>
        GroundedMinion
    }

    /// <summary>
    /// Base class for all summon-type cursed techniques — sentries and minions alike.
    /// Toggled on/off through the technique system. No buff/bool lifecycle needed.
    ///
    /// Subclasses must override:
    ///   - GetProjectileType() — return your own ModContent.ProjectileType
    ///   - SummonAI() — your actual behavior (targeting, attacking, visuals)
    ///   - All abstract CursedTechnique properties (Damage, Cost, etc.)
    ///
    /// Subclasses may override:
    ///   - SummonSetDefaults() — width, height, frames, etc.
    ///   - Any configuration properties below to customize behavior
    /// </summary>
    public abstract class CursedTechniqueSummon : CursedTechnique
    {
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  CONFIGURATION — Override these in subclasses
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━


        //Cleans up the minions if state switches
        //Important to set in sub classes unless it's a multi tech summon
        public virtual string ParentInnateName => null;


        /// <summary>
        /// Determines the movement style. Default Sentry.
        /// </summary>
        public virtual SummonStyle Style => SummonStyle.Sentry;

        /// <summary>
        /// Detection range for enemies, in pixels. Default 1000f.
        /// For minions, uses MinDetectionRange until a target is acquired, then expands to this.
        /// </summary>
        public virtual float DetectionRange => 1000f;

        /// <summary>
        /// Initial tighter detection range before a target is locked. Default 960f.
        /// Only used by FlyingMinion and GroundedMinion styles.
        /// </summary>
        public virtual float MinDetectionRange => 960f;

        // ── Sentry-Specific ───────────────────────────────────────────

        /// <summary>
        /// Whether a sentry collides with tiles (sits on ground). Default true.
        /// Only applies when Style = Sentry.
        /// </summary>
        public virtual bool SentryTileCollide => true;

        /// <summary>
        /// If true, a sentry hovers near the player instead of staying where placed.
        /// Only applies when Style = Sentry.
        /// </summary>
        public virtual bool SentryFollowsPlayer => false;

        /// <summary>
        /// Offset from the player center when SentryFollowsPlayer is true.
        /// </summary>
        public virtual Vector2 SentryFollowOffset => new Vector2(0, -60f);

        // ── Minion-Specific ───────────────────────────────────────────

        /// <summary>
        /// Local i-frame count for minion hits. Default 10.
        /// Multiplied by MaxUpdates internally.
        /// </summary>
        public virtual int IFrames => 10;

        /// <summary>
        /// Whether the minion can deal contact damage by touching enemies.
        /// Default false — subclass should fire shot projectiles instead.
        /// </summary>
        public virtual bool CanContactDamage => false;

        /// <summary>
        /// Distance from the player where the minion teleports back. Default 2000f.
        /// </summary>
        public virtual float TeleportDistance => 2000f;

        /// <summary>
        /// Distance from player that triggers "return to owner" behavior. Default 1300f.
        /// Doubles when a target is acquired.
        /// </summary>
        public virtual float LeashDistance => 1300f;

        // ── Grounded Minion ───────────────────────────────────────────

        /// <summary>
        /// Gravity acceleration for grounded minions and sentries. Default 0.4f.
        /// </summary>
        public virtual float Gravity => 0.4f;

        /// <summary>
        /// Max fall speed for grounded minions and sentries. Default 20f.
        /// </summary>
        public virtual float MaxFallSpeed => 20f;

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  STATE — Accessible from subclasses
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        /// <summary>Timer. Increments every tick. Stored in ai[0].</summary>
        public ref float SummonTimer => ref Projectile.ai[0];

        /// <summary>General-purpose state. Stored in ai[1]. Use for cooldowns, phases, etc.</summary>
        public ref float SummonState => ref Projectile.ai[1];

        /// <summary>General-purpose data. Stored in ai[2].</summary>
        public ref float SummonData => ref Projectile.ai[2];

        /// <summary>The player who owns this summon.</summary>
        public Player Owner { get; private set; }

        /// <summary>The SorceryFightPlayer of the owner.</summary>
        public SorceryFightPlayer SFOwner { get; private set; }

        /// <summary>The currently tracked enemy target. Null if none. Only set for minion styles by default.</summary>
        public NPC Target { get; private set; }

        /// <summary>Adaptive detection — tighter when no target, wider once locked.</summary>
        private float AdaptiveDetectionRange => Target == null ? MinDetectionRange : DetectionRange;

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  SETUP
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;

            if (Style != SummonStyle.Sentry)
            {
                Main.projPet[Type] = true;
                ProjectileID.Sets.DrawScreenCheckFluff[Type] = (int)DetectionRange;
            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 50;
            Projectile.height = 50;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.netImportant = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;

            switch (Style)
            {
                case SummonStyle.Sentry:
                    Projectile.sentry = true;
                    Projectile.tileCollide = SentryTileCollide;
                    break;

                case SummonStyle.FlyingMinion:
                    Projectile.minion = true;
                    Projectile.minionSlots = 0f;
                    Projectile.tileCollide = false;
                    Projectile.usesLocalNPCImmunity = true;
                    break;

                case SummonStyle.GroundedMinion:
                    Projectile.minion = true;
                    Projectile.minionSlots = 0f;
                    Projectile.tileCollide = true;
                    Projectile.usesLocalNPCImmunity = true;
                    break;
            }

            SummonSetDefaults();
        }

        /// <summary>
        /// Override this to set summon-specific defaults (width, height, etc.)
        /// without needing to repeat base.SetDefaults().
        /// </summary>
        public virtual void SummonSetDefaults() { }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  AI LOOP
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        public override void AI()
        {
            Owner = Main.player[Projectile.owner];
            SFOwner = Owner.SorceryFight();

            // Die if owner is dead or disconnected
            if (!Owner.active || Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            //Kill all minions when tech is null
            if (SFOwner?.innateTechnique?.Name == null)
            {
                Projectile.Kill();
                return;
            }

            //Kill minion if tech changes
            if (SFOwner.innateTechnique.Name != ParentInnateName)
            {
                Projectile.Kill();
                return;
            }

            // CE drain
            if (Projectile.owner == Main.myPlayer)
            {
                ActiveDrain(SFOwner);
            }

            // Style-specific base behavior
            switch (Style)
            {
                case SummonStyle.Sentry:
                    RunSentryBase();
                    break;

                case SummonStyle.FlyingMinion:
                    Projectile.localNPCHitCooldown = IFrames * Projectile.MaxUpdates;
                    SetTarget();
                    break;

                case SummonStyle.GroundedMinion:
                    Projectile.localNPCHitCooldown = IFrames * Projectile.MaxUpdates;
                    SetTarget();
                    ApplyGravity();
                    break;
            }

            SummonTimer++;

            // Delegate to subclass
            SummonAI();
        }

        /// <summary>
        /// Override this with your summon's actual behavior.
        /// Called every tick after base logic (gravity, targeting, etc.) runs.
        /// </summary>
        public virtual void SummonAI() { }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  STYLE-SPECIFIC BASE BEHAVIOR
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        private void RunSentryBase()
        {
            if (SentryFollowsPlayer)
            {
                Vector2 destination = Owner.Center + SentryFollowOffset;
                float bob = (float)Math.Sin(Projectile.timeLeft / 30f) * 4f;
                destination.Y += bob;

                Projectile.Center = Vector2.Lerp(Projectile.Center, destination, 0.08f);

                if (Projectile.WithinRange(destination, 4f) || !Projectile.WithinRange(destination, 2000f))
                    Projectile.Center = destination;

                Projectile.velocity = Vector2.Zero;
            }
            else if (SentryTileCollide)
            {
                Projectile.velocity.Y += 0.5f;
                if (Projectile.velocity.Y > 10f)
                    Projectile.velocity.Y = 10f;
            }
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  TOGGLE (UseTechnique)
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI != Main.myPlayer)
                return -1;

            int summonType = GetProjectileType();

            // Check for existing summon — toggle off
            bool found = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == summonType)
                {
                    proj.Kill();
                    found = true;
                }
            }

            if (found)
            {
                if (DisplayNameInGame)
                {
                    int idx = CombatText.NewText(player.getRect(), textColor, $"{DisplayName.Value} - Dismissed");
                    Main.combatText[idx].lifeTime = 120;
                }
                return -1;
            }

            // Deduct costs
            sf.cursedEnergy -= CalculateTrueCost(sf);

            if (BloodCost > 0)
                sf.bloodEnergy -= BloodCost;

            if (StarCost > 0)
                sf.starEnergy -= StarCost;

            // Display name
            if (DisplayNameInGame)
            {
                int idx = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                Main.combatText[idx].lifeTime = 180;
            }

            // Spawn
            Vector2 spawnPos = Main.MouseWorld;
            var source = player.GetSource_FromThis();

            int projIndex = Projectile.NewProjectile(
                source,
                spawnPos,
                Vector2.Zero,
                summonType,
                (int)CalculateTrueDamage(sf),
                0f,
                player.whoAmI
            );

            if (Main.projectile.IndexInRange(projIndex))
                Main.projectile[projIndex].originalDamage = Damage;

            if (Style == SummonStyle.Sentry)
                player.UpdateMaxTurrets();

            return projIndex;
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  DAMAGE
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        /// <summary>
        /// Sentry bodies don't deal contact damage by default.
        /// Minions can opt in via CanContactDamage.
        /// </summary>
        public override bool? CanDamage()
        {
            if (Style == SummonStyle.Sentry)
                return false;

            return null; // let MinionContactDamage decide
        }

        public override bool MinionContactDamage() => CanContactDamage;

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  TILE COLLISION
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Style == SummonStyle.GroundedMinion)
                Projectile.velocity.X *= 0.9f;

            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            if (Style == SummonStyle.GroundedMinion)
                fallThrough = Projectile.Bottom.Y < Owner.Top.Y;
            else
                fallThrough = false;

            return true;
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  NETWORK SYNC
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.ai[0]);
            writer.Write(Projectile.ai[1]);
            writer.Write(Projectile.ai[2]);
            writer.Write(Projectile.velocity.X);
            writer.Write(Projectile.velocity.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.ai[0] = reader.ReadSingle();
            Projectile.ai[1] = reader.ReadSingle();
            Projectile.ai[2] = reader.ReadSingle();
            Projectile.velocity.X = reader.ReadSingle();
            Projectile.velocity.Y = reader.ReadSingle();
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  TARGETING
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        /// <summary>
        /// Finds the best target. Prioritizes the player's whip/manual target,
        /// then falls back to the closest valid NPC within range.
        /// Called automatically for minion styles. Sentries should call manually if needed.
        /// </summary>
        public void SetTarget()
        {
            float range = Style == SummonStyle.Sentry ? DetectionRange : AdaptiveDetectionRange;

            if (Owner.HasMinionAttackTargetNPC)
            {
                NPC manual = Main.npc[Owner.MinionAttackTargetNPC];
                if (manual.CanBeChasedBy(Projectile) &&
                    Vector2.Distance(Projectile.Center, manual.Center) < range)
                {
                    Target = manual;
                    return;
                }
            }

            NPC best = null;
            float bestDist = range;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile))
                    continue;

                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = npc;
                }
            }

            Target = best;
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  MOVEMENT HELPERS
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        /// <summary>
        /// Apply gravity for grounded summons. Call from SummonAI if needed.
        /// Already called automatically for GroundedMinion style.
        /// </summary>
        protected void ApplyGravity()
        {
            if (Projectile.velocity.Y < MaxFallSpeed)
                Projectile.velocity.Y = MathF.Min(Projectile.velocity.Y + Gravity, MaxFallSpeed);
        }

        /// <summary>
        /// Flying minion follow behavior. Hovers near the player, turns back
        /// when drifting, teleports if extremely far.
        /// </summary>
        protected void FlyNearOwner(float hoverDistance = 160f, float returnSpeed = 17f, float smoothing = 40f)
        {
            if (Projectile.WithinRange(Owner.Center, DetectionRange) && !Projectile.WithinRange(Owner.Center, 300f))
                Projectile.velocity = (Owner.Center - Projectile.Center) / 30f;
            else if (!Projectile.WithinRange(Owner.Center, hoverDistance))
                Projectile.velocity = (Projectile.velocity * smoothing + (Owner.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * returnSpeed) / (smoothing + 1f);

            if (!Projectile.WithinRange(Owner.Center, TeleportDistance))
            {
                Projectile.Center = Owner.Center;
                Projectile.velocity *= 0.3f;
                Projectile.netUpdate = true;
            }
        }

        /// <summary>
        /// Fly toward the current target, backing off if too close.
        /// </summary>
        protected void FlyTowardTarget(float approachSpeed = 18f, float retreatSpeed = 9f, float preferredDist = 200f, float smoothing = 40f)
        {
            if (Target == null)
                return;

            Vector2 toTarget = Target.Center - Projectile.Center;
            float dist = toTarget.Length();
            toTarget.Normalize();

            if (dist > preferredDist)
                toTarget *= approachSpeed;
            else
                toTarget *= -retreatSpeed;

            Projectile.velocity = (Projectile.velocity * smoothing + toTarget) / (smoothing + 1f);
        }

        /// <summary>
        /// Prevents multiple minions from stacking on each other.
        /// </summary>
        protected void AntiClump(float force = 0.05f)
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];
                if (i != Projectile.whoAmI && other.active && other.owner == Projectile.owner && other.type == Projectile.type &&
                    Vector2.Distance(Projectile.Center, other.Center) < Projectile.width)
                {
                    Vector2 pushDir = (Projectile.Center - other.Center).SafeNormalize(Vector2.UnitX);
                    Projectile.velocity += pushDir * force;
                    other.velocity -= pushDir * force;
                }
            }
        }

        /// <summary>
        /// Checks if the minion is too far from the player and should return.
        /// Uses SummonData (ai[2]) as the return flag.
        /// Returns true if the minion should be heading back to the player.
        /// </summary>
        protected bool ShouldReturnToOwner()
        {
            float leash = Target != null ? LeashDistance * 2f : LeashDistance;

            if (!Projectile.WithinRange(Owner.Center, leash))
            {
                SummonData = 1f;
                Projectile.netUpdate = true;
            }

            if (SummonData == 1f && Projectile.WithinRange(Owner.Center, LeashDistance * 0.5f))
            {
                SummonData = 0f;
                Projectile.netUpdate = true;
            }

            return SummonData == 1f;
        }

        /// <summary>
        /// Finds the closest NPC within a given range from the projectile.
        /// Standalone helper for sentries that don't use the automatic targeting.
        /// </summary>
        protected NPC FindClosestNPC(float range)
        {
            NPC best = null;
            float bestDist = range;

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.CanBeChasedBy(Projectile))
                    continue;

                float dist = Vector2.Distance(Projectile.Center, npc.Center);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = npc;
                }
            }

            return best;
        }

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  ANIMATION HELPERS
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        /// <summary>
        /// Simple frame cycling. Call from SummonAI.
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

        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
        //  UTILITY
        // ━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

        /// <summary>
        /// Counts how many projectiles of a given type this player owns.
        /// </summary>
        protected int CountOwned(int projectileType)
        {
            int count = 0;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.owner == Projectile.owner && proj.type == projectileType)
                    count++;
            }
            return count;
        }
    }
}
