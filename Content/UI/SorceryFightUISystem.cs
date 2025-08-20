using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.UI.Dialog;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class SorceryFightUISystem : ModSystem
    {
        internal UserInterface sfInterface;
        internal SorceryFightUI sfUI;
        internal DialogUI dialogUI;

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
            if (!Main.dedServ)
            {
                sfInterface.SetState(null);
                sfUI = null;
                dialogUI = null;
            }
        }

        public override void OnWorldLoad()
        {
            if (!Main.dedServ)
            {
                sfUI = new SorceryFightUI();
                sfUI.Activate();

                sfInterface.SetState(sfUI);
            }
        }

        public override void OnWorldUnload()
        {
            sfUI = null;
            dialogUI = null;
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


        public void ActivateDialogUI(Dialog.Dialog dialog, object initiator)
        {
            if (Main.dedServ) return;

            SoundEngine.PlaySound(SoundID.MenuOpen, Main.LocalPlayer.Center);
            dialogUI = new DialogUI(dialog, initiator);
            dialogUI.Activate();
            sfInterface.SetState(dialogUI);
        }

        public void DeactivateDialogUI()
        {
            sfUI = new SorceryFightUI();
            sfUI.Activate();
            sfInterface.SetState(sfUI);
        }
    }
}