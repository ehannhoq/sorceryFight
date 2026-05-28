using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearerPunch(BossNPC bossNPC) : AIState(bossNPC)
    {
        private static readonly int FRAMES = 7;
        private static readonly int TICK_PER_FRAME = 7;
        private static readonly Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearerPunch", AssetRequestMode.ImmediateLoad).Value;

        public int frame;
        public int frametime;


        public override void AI(NPC npc)
        {
            if (frametime++ >= TICK_PER_FRAME - 1)
            {
                frametime = 0;
                if (frame++ >= FRAMES - 1)
                {
                    frame = FRAMES - 1;
                    Rectangle punchHitbox = new(
                        (int)(npc.direction == 1 ? npc.position.X + npc.width : npc.position.X - 100),
                        (int)npc.position.Y,
                        100,
                        npc.height
                    ); 
                    foreach (Player player in Main.ActivePlayers)
                    {
                        if (player.Hitbox.Intersects(punchHitbox))
                        {
                            player.Hurt(PlayerDeathReason.ByNPC(npc.whoAmI), npc.damage, npc.direction, dodgeable:true, knockback:5f);
                        }
                    }
                    bossNPC.SetState(new FingerBearerTrack(bossNPC));
                    return;
                }
            }
        }

        public override void OnEnter(NPC npc)
        {
            frame = 0;
            frametime = 0;
        }

        public override void OnExit(NPC npc)
        {
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