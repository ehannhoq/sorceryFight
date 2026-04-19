using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Projectiles;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearer : ModNPC
    {
        private enum AnimationState
        {
            Idle,
            Walk,
            Attack
        }

        private AnimationState currentAnimation = AnimationState.Idle;


        private static Texture2D idleTexture;
        private static Texture2D attackTexture;
        private static Texture2D walkTexture;
        private static Texture2D deathTexture;

        private int currentFrame;
        private int frameCounter;
        private int maxFrames;
        private int frameTime;
        private bool readyToAttack = false;

        private float attackRange = 450f;
        private float movementSpeed = 3f;


        public override void SetStaticDefaults()
        {
            idleTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearer", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            attackTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearerAttack", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            walkTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearerWalk", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            deathTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearerDeath", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void SetDefaults()
        {
            NPC.width = 90;
            NPC.height = 86;
            NPC.npcSlots = 6;
            NPC.defense = 12;
            NPC.damage = 120;
            NPC.netAlways = true;
            NPC.aiStyle = 0;
            NPC.lifeMax = 2000;
            NPC.knockBackResist = 0.5f;
            NPC.Hitbox = new Rectangle(0, 0, NPC.width, NPC.height);
        }

        public override void FindFrame(int frameHeight)
        {
            if (readyToAttack)
            {
                currentAnimation = AnimationState.Attack;
            }
            else if (NPC.HasValidTarget)
            {
                float distance = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center);

                if (distance < attackRange)
                {
                    if (currentAnimation != AnimationState.Attack)
                        currentFrame = 0;

                    currentAnimation = AnimationState.Attack;
                }
                else if (distance < attackRange * 2)
                {
                    if (currentAnimation != AnimationState.Walk)
                        currentFrame = 0;

                    currentAnimation = AnimationState.Walk;
                }
            }
            else
            {
                if (currentAnimation != AnimationState.Idle)
                    currentFrame = 0;

                currentAnimation = AnimationState.Idle;
            }

            switch (currentAnimation)
            {
                case AnimationState.Idle:
                    maxFrames = 6;
                    frameTime = 30;
                    break;
                case AnimationState.Walk:
                    maxFrames = 7;
                    frameTime = 8;
                    break;
                case AnimationState.Attack:
                    maxFrames = 5;
                    frameTime = 10;
                    break;
            }

            if (currentAnimation == AnimationState.Attack && readyToAttack)
            {
                currentFrame = maxFrames - 1;
                return;
            }

            if (++frameCounter >= frameTime)
            {
                frameCounter = 0;
                currentFrame++;

                if (currentAnimation == AnimationState.Attack)
                {
                    if (currentFrame >= maxFrames - 1)
                    {
                        currentFrame = maxFrames - 1;
                        readyToAttack = true;
                        NPC.ai[0] = 0;
                    }
                }
                else if (currentFrame >= maxFrames)
                {
                    currentFrame = 0;
                }
            }
        }

        public override void AI()
        {
            NPC.TargetClosest();

            if (NPC.HasValidTarget)
            {
                if (currentAnimation == AnimationState.Walk)
                {
                    Vector2 toPlayer = NPC.Center.DirectionTo(Main.player[NPC.target].Center);
                    Vector2 xVector = Vector2.UnitX * NPC.direction;

                    float dotprod = Vector2.Dot(toPlayer, xVector);
                    Vector2 projection = dotprod / xVector.LengthSquared() * xVector;

                    NPC.velocity.X = (projection * movementSpeed).X;
                }
                else
                {
                    NPC.velocity.X = 0f;
                }
            }

            if (readyToAttack && NPC.HasValidTarget)
            {
                NPC.ai[0]++;
                int chargeUpTime = 60;

                if (NPC.ai[0] == 1 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(
                        NPC.GetSource_FromThis(),
                        NPC.Center - new Vector2(0, 50),
                        Vector2.Zero,
                        ModContent.ProjectileType<FingerBearerBall>(),
                        20,
                        2f,
                        default,
                        default,
                        chargeUpTime,
                        NPC.target
                    );
                }

                if (NPC.ai[0] >= chargeUpTime)
                {
                    readyToAttack = false;
                    NPC.ai[0] = 0;
                    currentFrame = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.bloodMoon && !Main.dayTime && spawnInfo.Player.ZoneOverworldHeight && Main.hardMode)
                return 0.01f;
            return 0f;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SukunasFingerIV>()));
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture;
            int frames;

            switch (currentAnimation)
            {
                case AnimationState.Idle:
                    texture = idleTexture;
                    frames = 6;
                    break;
                case AnimationState.Walk:
                    texture = walkTexture;
                    frames = 7;
                    break;
                case AnimationState.Attack:
                    texture = attackTexture;
                    frames = 5;
                    break;
                default:
                    texture = TextureAssets.Npc[Type].Value;
                    frames = 1;
                    break;
            }

            int frameHeight = texture.Height / frames;
            Rectangle src = new Rectangle(0, currentFrame * frameHeight, texture.Width, frameHeight);

            SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, src, drawColor, NPC.rotation, src.Size() * 0.5f, NPC.scale * 2f, spriteEffects, 0f);
            return false;
        }
    }
}