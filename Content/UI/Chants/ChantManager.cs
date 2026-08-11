using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace sorceryFight.Content.UI.Chants
{
    public class ChantManager : ModSystem
    {
        private static Chant ActiveChant;
        private static Action queuedOnEnd;

        public static int InitiateChant(Chant chant)
        {
            ActiveChant = chant;
            return chant.totalTime;
        }

        public void DrawChant(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.UIScaleMatrix
            );
            ActiveChant?.Draw(Main.spriteBatch);
            Main.spriteBatch.End();
        }

        public override void PostUpdateEverything()
        {
            if (queuedOnEnd != null)
            {
                Action toRun = queuedOnEnd;
                queuedOnEnd = null;
                toRun.Invoke();
                return;
            }

            if (ActiveChant != null)
            {
                ActiveChant.Update();
                ActiveChant.tick++;

                if (ActiveChant.tick >= ActiveChant.totalTime)
                {
                    queuedOnEnd = ActiveChant.onEnd;
                    ActiveChant = null;
                }
            }
        }

        public override void Load()
        {
            On_Main.DrawPlayers_AfterProjectiles += DrawChant;
        }

        public override void Unload()
        {
            On_Main.DrawPlayers_AfterProjectiles -= DrawChant;
        }
    }
}
