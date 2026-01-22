using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace sorceryFight.Content.Quests
{
    public abstract class Quest
    {
        /// <summary>
        /// The display name of this quest. Is automatically set by the class name. Be sure that the class name is the same as the localization key.
        /// </summary>
        public string DisplayName { get; private set; }


        /// <summary>
        /// The description of this quest. Is automatically set by the class name. Be sure that the class name is the same as the localization key.
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Initializes the quest's <paramref name="DisplayName"/> and <paramref name="Description"/>.
        /// </summary>
        public void Initialize()
        {
            DisplayName = SFUtils.GetLocalizationValue($"Mods.sorceryFight.Quests.{this.GetClass()}.DisplayName");
            Description = SFUtils.GetLocalizationValue($"Mods.sorceryFight.Quests.{this.GetClass()}.Description");
        }


        /// <summary>
        /// Whether or not this quest is currently available for the player. Defaults to true.
        /// </summary>
        public virtual bool IsAvailable(SorceryFightPlayer sfPlayer) => true;

        /// <summary>
        /// Used to load any variables needed for the quest.
        /// </summary>
        public virtual void OnAddedQuest(SorceryFightPlayer sfPlayer) { }

        /// <summary>
        /// Used to unload any variables that may linger on the player.
        /// </summary>
        public virtual void OnCompletedQuest(SorceryFightPlayer sfPlayer) { }

        /// <summary>
        /// Gets called when the player uses an item.
        /// </summary>
        public virtual void UsedItem(SorceryFightPlayer sfPlayer, Item item) { }

        /// <summary>
        /// Determines if the player has successfully completed this quest.
        /// </summary>
        public abstract bool IsCompleted(SorceryFightPlayer sfPlayer);

        public static Quest QuestBuilder(string typeName)
        {
            foreach (var type in AssemblyManager.GetLoadableTypes(ModContent.GetInstance<SorceryFight>().Code))
            {
                if (type.IsAbstract || !typeof(Quest).IsAssignableFrom(type))
                    continue;

                if (Activator.CreateInstance(type) is Quest quest && quest.GetClass() == typeName)
                {
                    quest.Initialize();
                    return quest;
                }
            }

            throw new Exception($"No quest found with type {typeName}");
        }
    }
}