using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.Mahoraga
{
    public class MahoragaWalk(BossNPC bossNPC) : AIState(bossNPC)
    {
        public override void AI(NPC npc)
        {
            Vector2 toPlayer = npc.Center.DirectionTo(bossNPC.GetTarget().Center);
            Vector2 xVector = Vector2.UnitX * npc.direction;

            float dotprod = Vector2.Dot(toPlayer, xVector);
            Vector2 projection = dotprod / xVector.LengthSquared() * xVector;

            npc.velocity.X = (projection * MahoragaBoss.MOVEMENT_SPEED).X;

            Collision.StepUp(ref npc.position, ref npc.velocity, npc.width, npc.height, ref npc.stepSpeed, ref npc.gfxOffY);
        }

        public override void OnEnter(NPC npc)
        {
            MahoragaBoss mahoraga = bossNPC as MahoragaBoss;

            mahoraga.topSprite = new NPCSpritePart(
                sprite: ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Mahoraga/MahoragaWalk_Upper", AssetRequestMode.ImmediateLoad).Value,
                frames: 6,
                ticksPerFrame: 5
            );
            mahoraga.bottomSprite = new NPCSpritePart(
                sprite: ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Mahoraga/MahoragaWalk_Lower", AssetRequestMode.ImmediateLoad).Value,
                frames: 6,
                ticksPerFrame: 5
            );
        }

        public override void OnExit(NPC npc) { }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) { return false; }
    }
}