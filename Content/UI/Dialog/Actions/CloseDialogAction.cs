using sorceryFight.Content.NPCs.TownNPCs;
using sorceryFight.Content.Quests;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.UI.Dialog.Actions
{
    public class CloseDialogAction : IAction
    {
        public string uiText;

        private object initiator;


        public CloseDialogAction(string uiText)
        {
            this.uiText = uiText;
        }


        public void Invoke()
        {
            if (Main.dedServ) return;
            ModContent.GetInstance<SorceryFightUISystem>().ResetUI();
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