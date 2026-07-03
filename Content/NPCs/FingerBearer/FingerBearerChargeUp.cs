using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using sorceryFight.Content.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearerChargeUp(BossNPC bossNPC) : AIState(bossNPC)
    {
        private static readonly Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearerChargeUp", AssetRequestMode.ImmediateLoad).Value;

        private static int FRAMES = 5;
        private static int TICKS_PER_FRAME = 10;
        private int frame;
        private int frametime;
        private int tick;
        private bool alreadyDashed;
        private const int chargeUpTime = 60;
        private int projectile;

        private Player target;

        public override void AI(NPC npc)
        {
            if (frametime++ >= TICKS_PER_FRAME)
            {
                if (frame < FRAMES - 1)
                {
                    frame++;
                }
            }

            HandleProjectile(npc);
            HandleRunningAway(npc);

            if (projectile != -1)
            {
                Main.projectile[projectile].Center = npc.Center - new Vector2(0, 50);
            }

            if (tick >= chargeUpTime + 30)
            {
                bossNPC.SetState(new FingerBearerDefaultState(bossNPC));
            }
        }


        private void HandleProjectile(NPC npc)
        {
            tick++;
            if (frame == FRAMES - 2)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    projectile = Projectile.NewProjectile(
                        npc.GetSource_FromThis(),
                        npc.Center - new Vector2(0, 50),
                        Vector2.Zero,
                        ModContent.ProjectileType<FingerBearerBall>(),
                        20,
                        2f,
                        default,
                        default,
                        chargeUpTime,
                        npc.target
                    );
                }
            }
        }

        private void HandleRunningAway(NPC npc)
        {
            if (alreadyDashed) return;

            foreach (Player player in Main.ActivePlayers)
            {
                Vector2 targetToNPC = (npc.Center - player.Center).SafeNormalize(Vector2.UnitX);
                Vector2 playerDir = player.velocity.SafeNormalize(Vector2.UnitX);
                float distance = (npc.Center - player.Center).Length();

                if (Vector2.Dot(targetToNPC, playerDir) >= 0.6 && player.velocity.Length() > 7.0f)
                {
                    npc.target = player.whoAmI;
                    Main.projectile[projectile].ai[2] = player.whoAmI;

                    float heightDifference = npc.height - player.height;
                    npc.Center = (player.Center - new Vector2(0.0f, heightDifference)) + targetToNPC * -distance;
                    alreadyDashed = true;
                    return;
                }
            }
        }

        public override void OnEnter(NPC npc)
        {
            frame = 0;
            frametime = 0;
            tick = 0;
            alreadyDashed = false;
            projectile = -1;
            ((FingerBearer)bossNPC).readyForOrb = false;
        }


        public override void OnExit(NPC npc)
        {
            tick = 0;
            target = null;
            projectile = -1;
            ((FingerBearer)bossNPC).readyForOrb = true;
        }


        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = texture.Height / FRAMES;
            Rectangle src = new Rectangle(0, frame * frameHeight, texture.Width, frameHeight);

            SpriteEffects spriteEffects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, src, drawColor, npc.rotation, src.Size() * 0.5f, npc.scale * 2f, spriteEffects, 0f);
            return false;
        }
    }
}
