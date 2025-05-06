using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public class DomainExpansionController : ModSystem
    {
        public static Dictionary<int, DomainExpansion> ActiveDomains = new Dictionary<int, DomainExpansion>();

        public override void PostUpdateNPCs()
        {
            foreach (var item in ActiveDomains)
            {
                var value = item.Value;

                value.Update();
            }
        }

        public override void Load()
        {
            IL_Main.DrawNPCs += DrawDomainsBeforeNPCs;
        }

        public override void Unload()
        {
            IL_Main.DrawNPCs -= DrawDomainsBeforeNPCs;
        }

        private void DrawDomainsBeforeNPCs(ILContext il)
        {
            ILCursor cursor = new ILCursor(il);

            cursor.EmitDelegate(() =>
            {
                DrawActiveDomains(Main.spriteBatch);
            });
        }

        public static void DrawActiveDomains(SpriteBatch spriteBatch)
        {
            if (ActiveDomains.Count == 0) return;

            Main.spriteBatch.End();

            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            foreach (var domain in ActiveDomains.Values)
            {
                domain.Draw(spriteBatch);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        }
    }
}
