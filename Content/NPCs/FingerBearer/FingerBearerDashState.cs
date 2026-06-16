using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.FingerBearer
{
    public class FingerBearerDashState(BossNPC bossNPC, Vector2 targetPos) : AIState(bossNPC)
    {
        private static readonly Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/FingerBearer/FingerBearerDash", AssetRequestMode.ImmediateLoad).Value;

        float tick;
        Vector2 startPos;

        public override void AI(NPC npc)
        {
            tick++;
            float t = MathHelper.Clamp(tick / 30.0f, 0f, 1f);
            float eased = t < 0.5f
                ? 2f * t * t
                : 1f - MathF.Pow(-2f * t + 2f, 2f) / 2f;

            npc.Center = Vector2.Lerp(startPos, targetPos, eased);

            if ((npc.Center - targetPos).Length() < 5f)
                bossNPC.SetState(new FingerBearerTrack(bossNPC));
        }

        public override void OnEnter(NPC npc)
        {
            tick = 0;
            startPos = npc.Center;
            npc.noTileCollide = true;
        }

        public override void OnExit(NPC npc)
        {
            npc.noTileCollide = false;
        }

        public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
            SpriteEffects spriteEffects = npc.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(texture, npc.Center - Main.screenPosition, src, drawColor, npc.rotation, src.Size() * 0.5f, npc.scale * 2f, spriteEffects, 0f);
            return false;
        }
    }
}