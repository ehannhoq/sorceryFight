using Humanizer;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.VFX
{
    public class VFXManager : ModSystem
    {
        public static VFXObject[] vfxs;
        public override void Load()
        {
            vfxs = new VFXObject[1024];
            On_Main.DrawPlayers_BehindNPCs += DrawBehindNPCS;
            On_Main.DrawPlayers_AfterProjectiles += DrawAboveNPCS;
        }

        public override void Unload()
        {
            vfxs = null;
            On_Main.DrawPlayers_BehindNPCs -= DrawBehindNPCS;
            On_Main.DrawPlayers_AfterProjectiles -= DrawAboveNPCS;

        }

        public override void PostUpdateEverything()
        {
            for (int i = 0; i < vfxs.Length; i++)
            {
                if (vfxs[i] == null) continue;
                VFXObject vfx = vfxs[i];

                vfx.tick++;
                vfx.Update();

                if (vfx.tick >= vfx.lifetime)
                {
                    vfxs[i] = null;
                }
            }
        }

        private void DrawBehindNPCS(On_Main.orig_DrawPlayers_BehindNPCs orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            for (int i = 0; i < vfxs.Length; i++)
            {
                if (vfxs[i] == null) continue;
                VFXObject vfx = vfxs[i];

                if (vfx.drawLayer == VFXDrawLayer.BehindNPCs)
                    vfx.Draw(Main.spriteBatch);
            }

            Main.spriteBatch.End();
        }

        private void DrawAboveNPCS(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            for (int i = 0; i < vfxs.Length; i++)
            {
                if (vfxs[i] == null) continue;
                VFXObject vfx = vfxs[i];

                if (vfx.drawLayer == VFXDrawLayer.AboveNPCs)
                    vfx.Draw(Main.spriteBatch);
            }

            Main.spriteBatch.End();
        }

        public static ref VFXObject AddVFX(VFXObject vfx)
        {
            int index = vfxs.Append(vfx);
            return ref vfxs[index];
        }
    }
}