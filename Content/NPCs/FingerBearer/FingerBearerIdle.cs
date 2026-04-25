using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearerDefaultState : IAIState
    {
        private static int IDLE_FRAMES = 6;
        private static int TICK_PER_FRAME = 30;
        private static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearer", AssetRequestMode.ImmediateLoad).Value;

        public void AI(NPC npc)
        {
            throw new System.NotImplementedException();
        }

        public void OnEnter(NPC npc)
        {
            throw new System.NotImplementedException();
        }

        public void OnExit(NPC npc)
        {
            throw new System.NotImplementedException();
        }

        public bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            throw new System.NotImplementedException();
        }
    }
}