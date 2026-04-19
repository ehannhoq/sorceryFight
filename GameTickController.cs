using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content
{
    public class GameTickController : ModSystem
    {
        private static float GameSpeed = 1.0f;
        public override void Load()
        {
            // On_Main.DoUpdate += DoUpdate;
        }

        public override void PostUpdateEverything()
        {
            GameSpeed = 1.0f;
        }

        private void DoUpdate(On_Main.orig_DoUpdate orig, Main self, ref GameTime gameTime)
        {
            var scaledTime = new GameTime(
                gameTime.TotalGameTime,
                TimeSpan.FromTicks((long)(gameTime.ElapsedGameTime.Ticks * GameSpeed))
            );
            orig(self, ref scaledTime);
        }
    }
}