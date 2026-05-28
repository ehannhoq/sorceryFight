using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs
{
    [Autoload(false)]
    public abstract class BossNPC : ModNPC
    {
        public AIState currentState;

        public override void SetDefaults()
        {
            NPC.boss = true;
            NPC.friendly = false;
        }

        public override void AI()
        {
            if (currentState == null) return;

            currentState?.AI(NPC);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (currentState == null) return true;

            return currentState.PreDraw(NPC, spriteBatch, screenPos, drawColor);
        }

        public void SetState(AIState newState)
        {
            currentState?.OnExit(NPC);
            currentState = newState;
            currentState.OnEnter(NPC);
        }

        public float GetDistanceToTarget()
        {
            if (!NPC.HasValidTarget) return -1f;

            return (NPC.Center - Main.player[NPC.target].Center).Length();
        }

        public Player GetTarget()
        {
            if (!NPC.HasValidTarget) return null;
            return Main.player[NPC.target];
        }
    }
}