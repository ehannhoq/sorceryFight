using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace sorceryFight.Content.NPCs
{
    public interface IAIState
    {
        public void AI(NPC npc);

        public bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor);

        public void OnEnter(NPC npc);

        public void OnExit(NPC npc);
    }
}   