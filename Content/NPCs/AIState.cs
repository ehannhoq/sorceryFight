using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace sorceryFight.Content.NPCs
{
    public abstract class AIState(BossNPC bossNPC)
    {
        public BossNPC bossNPC = bossNPC;

        public abstract void AI(NPC npc);

        public abstract bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);

        public abstract void OnEnter(NPC npc);

        public abstract void OnExit(NPC npc);
    }
}   