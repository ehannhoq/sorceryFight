using System.Linq;
using sorceryFight.Content.NPCs.TownNPCs;
using sorceryFight.Content.Quests;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.UI.Dialog
{
    public class QueryQuestAction : IAction
    {
        public string uiText;

        private object initiator;


        public QueryQuestAction(string uiText)
        {
            this.uiText = uiText;
        }


        public void Invoke()
        {
            if (Main.dedServ) return;

            if (initiator is SorceryFightNPC sfNPC)
            {
                SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();
                SorceryFightUISystem uiSystem = ModContent.GetInstance<SorceryFightUISystem>();

                if (sfNPC.GetQuestIfAvailable(sfPlayer, out Quest quest))
                {
                    if (sfPlayer.currentQuests.Any(q => q.GetClass() == quest.GetClass()))
                    {
                        uiSystem.ActivateDialogUI(Dialog.Create($"{sfNPC.name}.ActiveQuest"), initiator);
                        return;
                    }

                    uiSystem.ActivateDialogUI(Dialog.Create($"{sfNPC.name}.{quest.GetClass()}"), initiator);
                }
                else
                    uiSystem.ActivateDialogUI(Dialog.Create($"{sfNPC.name}.NoQuests"), initiator);
            }
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