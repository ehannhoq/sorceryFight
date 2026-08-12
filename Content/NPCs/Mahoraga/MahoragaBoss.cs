using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.Mahoraga
{
    [Autoload(true)]
    [AutoloadBossHead]
    public class MahoragaBoss : BossNPC
    {
        internal const float MOVEMENT_SPEED = 5.0f;
        internal const float MINIMUM_DISTANCE_TO_PLAYER = 1000f;
        internal const int ACCUMULATIVE_DAMAGE_TIME = 60;

        internal bool waitingForAccumulativeDamageAssessment;
        internal Dictionary<int, int> accumulativeProjectileDamage;
        internal Dictionary<int, int> accumulativeItemDamage;
        internal PriorityQueue<AdaptationInfo, float> adaptationQueue;

        internal List<AdaptationInfo> adaptedProjectiles;
        internal List<AdaptationInfo> adaptedItems;

        internal MahoragaWheel wheel;
        internal NPCSpritePart topSprite;
        internal NPCSpritePart bottomSprite;

        private bool DrawNPCParts => currentState is not MahoragaIdle && topSprite.sprite != null && bottomSprite.sprite != null;


        public override void SetDefaults()
        {
            NPC.width = 120;
            NPC.height = 120;
            NPC.npcSlots = 12;
            NPC.defense = 60;
            NPC.damage = 300;
            NPC.netAlways = true;
            NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
            NPC.lifeMax = 120000;
            NPC.knockBackResist = 0.2f;
            currentState = new MahoragaIdle(this);
        }


        public override void OnSpawn(IEntitySource source)
        {
            waitingForAccumulativeDamageAssessment = false;
            accumulativeProjectileDamage = [];
            accumulativeItemDamage = [];

            adaptationQueue = new PriorityQueue<AdaptationInfo, float>(
                Comparer<float>.Create((x, y) => y.CompareTo(x))
            );
            
            adaptedProjectiles = [];
            adaptedItems = [];

            wheel = new MahoragaWheel(this);
        }

        public override bool PreAI()
        {
            if (!waitingForAccumulativeDamageAssessment)
                waitingForAccumulativeDamageAssessment = TaskScheduler.Instance.AddDelayedTask(AssessAccumulatedDamage, ACCUMULATIVE_DAMAGE_TIME);

            return true;
        }

        public override void AI()
        {
            int whoAmI = NPC.FindClosestPlayer(out float distanceToPlayer);
            NPC.target = distanceToPlayer <= MINIMUM_DISTANCE_TO_PLAYER ? whoAmI : -1;

            if (NPC.target == -1)
            {
                SetState(new MahoragaIdle(this));
            }


            if (DrawNPCParts)
            {
                if (topSprite.frameTime++ >= topSprite.ticksPerFrame)
                {
                    topSprite.frameTime = 0;
                    if (topSprite.frame++ >= topSprite.frames - 1)
                    {
                        topSprite.frame = 0;
                    }
                }

                if (bottomSprite.frameTime++ >= bottomSprite.ticksPerFrame)
                {
                    bottomSprite.frameTime = 0;
                    if (bottomSprite.frame++ >= bottomSprite.frames - 1)
                    {
                        bottomSprite.frame = 0;
                    }
                }
            }

            wheel.Update();
            Adapt();
            base.AI();
        }


        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) => AccumulateDamage(ref accumulativeProjectileDamage, projectile.type, damageDone);
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) => AccumulateDamage(ref accumulativeItemDamage, item.type, damageDone);
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers) => HandleAdaptedDamage(ref adaptedProjectiles, projectile.type, ref modifiers);
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers) => HandleAdaptedDamage(ref adaptedItems, item.type, ref modifiers);
    
 
        private void AccumulateDamage(ref Dictionary<int, int> accumulatedDamage, int damageType, int damage)
        {
            if (!accumulatedDamage.ContainsKey(damageType))
                accumulatedDamage[damageType] = 0;
            accumulatedDamage[damageType] += damage;
        }


        private void AssessAccumulatedDamage()
        {
            foreach (var kvp in accumulativeProjectileDamage)
            {
                AdaptationInfo info = new()
                {
                    damageSource = 0,
                    damageType = kvp.Key,
                };

                adaptationQueue.Enqueue(info, kvp.Value / (float)NPC.lifeMax);
            }

            foreach (var kvp in accumulativeItemDamage)
            {
                AdaptationInfo info = new()
                {
                    damageSource = 1,
                    damageType = kvp.Key
                };

                adaptationQueue.Enqueue(info, kvp.Value / (float)NPC.lifeMax);
            }

            waitingForAccumulativeDamageAssessment = false;
        }


        private void Adapt()
        {
            AdaptationInfo head = adaptationQueue.Peek();

            // static int f(int x) {
            //     if (x < 50)
            //         return (int)MathF.Round(4.5f * MathF.Cos(MathF.PI * x / 50) + 5.5f, 0);
            //     return 1;
            // };

            // static int g(int x) {
            //     return -10 * x + 110;
            // }

            // int percentDamageDone = (int)(damage / (float)NPC.lifeMax * 100);

            // AdaptationInfo info = adaptationInfo.Find(info => info.damageType == damageType);

            // if (info == null)
            // {
            //     info = new AdaptationInfo();
            //     info.damageType = damageType;
            //     info.currentAdaptationTime = f(percentDamageDone);
            //     // 10s -> 10%
            //     // 9s -> 20%
            //     // 8s -> 30%
            //     // 7s -> 40%
            //     // 6s -> 50%
            //     // 5s -> 60%
            //     // 4s -> 70%
            //     // 3s -> 80%
            //     // 2s -> 90%
            //     // 1s -> 100%
            //     info.anticipatedDamageReduction = g(info.currentAdaptationTime);
            //     adaptationInfo.Add(info);
            // }
            // else if (info.currentAdaptationTime > 1)
            // {
            //     info.currentAdaptationTime--;
            //     info.anticipatedDamageReduction = g(info.currentAdaptationTime);
            //     info.adaptationTimer = 0;
            // }

            // Item item = new Item();
            // item.SetDefaults(damageType);

            // Projectile proj = new Projectile();
            // proj.SetDefaults(damageType);

            // if (info.currentAdaptationTime > 1)
            //     Main.NewText($"adapting to [{item.Name} or {proj.Name}]\ntime till wheel spin: {info.currentAdaptationTime}, anticipated dmg reduction: {info.anticipatedDamageReduction}");
            // else
            //     Main.NewText($"fully adapted to [{item.Name} or {proj.Name}]");
        }


        private static void HandleAdaptedDamage(ref List<AdaptationInfo> adaptationInfo, int damageType, ref NPC.HitModifiers modifiers)
        {
            AdaptationInfo info = adaptationInfo.Find(info => info.damageType == damageType);
            if (info != null)
            {
                modifiers.FinalDamage *= 1.0f - (info.trueDamageReduction / 100f);
            }
        }


        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (DrawNPCParts)
            {
                SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                int frameHeight = bottomSprite.sprite.Height / bottomSprite.frames;
                int frameY = frameHeight * bottomSprite.frame;
                Rectangle src = new Rectangle(0, frameY, bottomSprite.sprite.Width, frameHeight);

                spriteBatch.Draw(bottomSprite.sprite, NPC.Center - Main.screenPosition, src, Color.White, NPC.rotation, src.Size() * 0.5f, NPC.scale * 2f, spriteEffects, 0f);

                frameHeight = topSprite.sprite.Height / topSprite.frames;
                frameY = frameHeight * topSprite.frame;
                src = new Rectangle(0, frameY, topSprite.sprite.Width, frameHeight);

                spriteBatch.Draw(topSprite.sprite, NPC.Center - Main.screenPosition, src, Color.White, NPC.rotation, src.Size() * 0.5f, NPC.scale * 2f, spriteEffects, 0f);

                return false;
            }

            return base.PreDraw(spriteBatch, screenPos, drawColor);
        }


        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            wheel?.PostDraw(spriteBatch, screenPos, drawColor);
            base.PostDraw(spriteBatch, screenPos, drawColor);
        }


        public override void OnKill()
        {
            SorceryFightDownedBossSystem.downedMahoraga = true;
            SorceryFightNetcode.SyncWorld();
        }
    }
}
