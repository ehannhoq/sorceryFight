using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using sorceryFight.Content.Quests;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        private class ItemListener : GlobalItem
        {
            public override bool? UseItem(Item item, Player player)
            {
                if (Main.myPlayer != player.whoAmI) return base.UseItem(item, player);

                SorceryFightPlayer sfPlayer = player.SorceryFight();
                foreach (Quest quest in sfPlayer.currentQuests)
                {
                    quest.UsedItem(sfPlayer, item);
                }
                return base.UseItem(item, player);
            }
        }
        public List<Quest> currentQuests;
        public List<string> completedQuests;

        private Dictionary<string, object> questData;

        public void AddQuest(Quest quest)
        {
            currentQuests.Add(quest);
            currentQuests.Find(q => q == quest).OnAddedQuest(this);
        }

        public void ModifyQuestData(Quest quest, string key, object obj)
        {
            string source = quest.GetClass();
            questData[source + key] = obj;
        }

        public object GetQuestData(Quest quest, string key)
        {
            string source = quest.GetClass();
            return questData[source + key];
        }

        public bool TryGetQuestData(Quest quest, string key, [NotNullWhen(true)] out object obj)
        {
            string source = quest.GetClass();

            if (questData.ContainsKey(source + key))
            {
                obj = questData[source + key];
                return true;
            }
            obj = null;
            return false;
        }

        private void CheckQuests()
        {
            if (Main.dedServ) return;

            for (int i = 0; i < currentQuests.Count; i++)
            {
                Quest quest = currentQuests[i];
                if (quest.IsCompleted(this))
                {
                    CompleteQuest(ref i);
                }
            }
        }

        private void CompleteQuest(ref int index)
        {
            Main.NewText($"Completed Quest: {currentQuests[index].DisplayName}");

            completedQuests.Add(currentQuests[index].GetClass());
            RemoveAllQuestData(currentQuests[index]);
            currentQuests[index].OnCompletedQuest(this);

            currentQuests.RemoveAt(index);
            index--;
        }

        private void RemoveAllQuestData(Quest quest)
        {
            string source = quest.GetType().ToString();
            foreach (string key in questData.Keys)
            {
                if (key.Contains(source))
                    questData.Remove(key);
            }
        }
    }
}