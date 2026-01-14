using Terraria;

namespace sorceryFight.Content.UI.Dialog
{
    public class OpenShopAction : IAction
    {
        public string shopName;
        public string uiText;
        private object initiator;


        public OpenShopAction(string shopName, string uiText)
        {
            this.shopName = shopName;
            this.uiText = uiText;
        }


        public void Invoke()
        {
            Main.NewText("test");
        }


        public void SetInitiator(object initiator)
        {
            this.initiator = initiator;
        }


        public string GetUIText()
        {
            return uiText;
        }
    }
}