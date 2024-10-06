using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class SorceryFightUISystem : ModSystem
    {
        internal UserInterface sfInterface;
        internal SorceryFightUI sfUI;
        private GameTime _lastUpdateUiGameTime;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                sfInterface = new UserInterface();
                sfUI = new SorceryFightUI();
                sfUI.Activate();
                sfInterface.SetState(sfUI);
            }
        }

        public override void Unload()
        {
            sfInterface.SetState(null);
            sfUI = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (sfInterface?.CurrentState != null)
            {
                sfInterface.Update(gameTime);
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "SorceryFight: SF Interface",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && sfInterface?.CurrentState != null)
                        {
                            sfInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
    }
}