using sorceryFight.Content.Quests;
using sorceryFight.SFPlayer;
using Terraria;

namespace sorceryFight.Content.UI.Dialog.Actions
{
    public class GiveQuestAction : IAction
    {
        public string questName;
        public string uiText;

        private object initiator;


        public GiveQuestAction(string questName, string uiText)
        {
            this.questName = questName;
            this.uiText = uiText;
        }


        public void Invoke()
        {
            if (Main.dedServ) return;

            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();
            sfPlayer.AddQuest(
                Quest.QuestBuilder(questName)
            );
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