using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Shops;
using sorceryFight.Content.UI.Dialog;
using sorceryFight.Content.UI.Shop;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static sorceryFight.Content.UI.Quests.QuestToast.QuestToast;

namespace sorceryFight.Content.UI
{
    public class SorceryFightUISystem : ModSystem
    {
        internal UserInterface sfInterface;
        internal SorceryFightUI sfUI;
        internal DialogUI dialogUI;
        internal SorceryFightShopUI shopUI;

        internal bool shopUIOpen;

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
                shopUI = null;
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
            shopUI = null;
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

        public void ActivateShopUI(string shopName)
        {
            if (Main.dedServ) return;

            SoundEngine.PlaySound(SoundID.MenuOpen, Main.LocalPlayer.Center);
            SorceryFightShop shop = SorceryFightShopRegistrar.GetShop(shopName);
            shopUI = new SorceryFightShopUI(shop);
            shopUI.Activate();
            sfInterface.SetState(shopUI);
            shopUIOpen = true;
        }

        public void QuestToastNotification(string questName, QuestToastType type)
        {
            TaskScheduler.Instance.AddDelayedTask(() =>
            {
                if (sfInterface.CurrentState is SorceryFightUI)
                {
                    sfUI.QuestToastNotification(questName, type);
                }
            }, 1);
        }
        public void ResetUI()
        {
            sfInterface.SetState(sfUI);
            shopUIOpen = false;
        }
    }
}