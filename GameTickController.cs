using System;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content
{
    public class GameTickController : ModSystem
    {
        private static float GameSpeed = 1.0f;
        public override void Load()
        {
            On_Main.DoUpdate += DoUpdate;
        }

        public override void PostUpdateEverything()
        {
            GameSpeed = 1.0f;
            //if (SFKeybinds.Test.Current)
            //{
            //    GameSpeed = 0.5f;
            //}
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