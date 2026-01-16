using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.UI.Dialog
{
    public class OpenShopAction : IAction
    {
        public string shopName;
        public string uiText;


        public OpenShopAction(string shopName, string uiText)
        {
            this.shopName = shopName;
            this.uiText = uiText;
        }


        public void Invoke()
        {
            if (Main.dedServ) return;

            Main.playerInventory = true;
            ModContent.GetInstance<SorceryFightUISystem>().ActivateShopUI(shopName);
        }


        public void SetInitiator(object initiator)
        {
        }


        public string GetUIText()
        {
            return uiText;
        }
    }
}