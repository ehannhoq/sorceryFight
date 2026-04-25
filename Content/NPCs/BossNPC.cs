using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs
{
    [AutoloadBossHead]
    public class BossNPC : ModNPC
    {
        public IAIState currentState;
        
        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.friendly = false;
        }

        public override void AI()
        {
            currentState.AI(NPC);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            return currentState.PreDraw(spriteBatch, screenPos, drawColor);
        }

        public void SetState(IAIState newState)
        {
            currentState.OnExit(NPC);
            currentState = newState;
            currentState.OnEnter(NPC);
        }
    }
}