using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;

using sorceryFight.Content.VFX;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class KamutokeLightning : ModProjectile
    {
        public static readonly int STARTING_RECURSIVE_LIGHTING_COUNT = 3;
        public static readonly int MINIMUM_NPC_DISTANCE = 400;

        private static Dictionary<int, List<int>> hitTracker;
        private static Dictionary<int, int> hitTrackerRefCount;

        private const int FRAMES = 3;
        private const int TICKS_PER_FRAME = 3;

        private Vector2 lightningBottomPosition => Projectile.Center + new Vector2(0f, Projectile.height / 2f);
        private ref float recursionCount => ref Projectile.ai[0];
        private ref float hitTrackerID => ref Projectile.ai[1];

        private bool reverseFrames = false;

        public override void Load()
        {
            hitTracker = [];
            hitTrackerRefCount = [];
        }

        public override void Unload()
        {
            hitTracker = null;
            hitTrackerRefCount = null;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 160;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.scale = 2f;
            Projectile.timeLeft = (FRAMES * 2) * TICKS_PER_FRAME;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Center -= new Vector2(0f, Projectile.height / 2f);

            if (recursionCount == STARTING_RECURSIVE_LIGHTING_COUNT)
            {
                hitTrackerID = hitTracker.Keys.Count;
                hitTracker[(int)hitTrackerID] = [];
                hitTrackerRefCount[(int)hitTrackerID] = 0;

                NPC anchor = Main.npc.OrderBy(npc => (npc.Center - lightningBottomPosition).Length()).FirstOrDefault();
                if (anchor != null && anchor.active)
                    hitTracker[(int)hitTrackerID].Add(anchor.whoAmI);
            }

            hitTrackerRefCount[(int)hitTrackerID]++;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (!reverseFrames)
                    Projectile.frame++;
                else
                    Projectile.frame--;

                if (Projectile.frame >= FRAMES - 1)
                {
                    reverseFrames = true;
                    VFXManager.AddVFX(new ImpactCircleVFX(lightningBottomPosition, lifetime: 60, scale: 2f, color: new Color(207, 140, 255, 255)));

                    if (recursionCount == 0) return;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            float distance = (npc.Center - lightningBottomPosition).Length();

                            if (distance > MINIMUM_NPC_DISTANCE) continue;
                            if (hitTracker[(int)hitTrackerID].Contains(npc.whoAmI)) continue;

                            hitTracker[(int)hitTrackerID].Add(npc.whoAmI);

                            Projectile.NewProjectile(Main.LocalPlayer.GetSource_FromThis(), npc.Center + new Vector2(0f, npc.height / 2f), Vector2.Zero, Projectile.type, Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: recursionCount - 1, ai1: hitTrackerID);
                        }
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = texture.Height / FRAMES;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (Projectile.frame < FRAMES - 1)
                return false;

            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            hitTracker[(int)hitTrackerID].Add(target.whoAmI);

            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Main.rand.NextFloat() * 20f;
                velocity.Y = -MathF.Abs(velocity.Y);
                LinearParticle particle = new LinearParticle(
                    target.Center + new Vector2(0f, target.height / 2f),
                    velocity,
                    new Color(207, 140, 255, 255),
                    lifetime: 30
                );
                ParticleController.SpawnParticle(particle);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (--hitTrackerRefCount[(int)hitTrackerID] <= 0)
            {
                hitTracker.Remove((int)hitTrackerID);
                hitTrackerRefCount.Remove((int)hitTrackerID);
            }
        }
    }
}   