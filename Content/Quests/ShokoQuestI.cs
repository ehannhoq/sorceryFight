using System;
using System.Linq;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Quests
{
    public class ShokoQuestI : Quest
    {
        private const int HEALTH_POTIONS = 2;
        public override bool IsCompleted(SorceryFightPlayer sfPlayer)
        {
            if (!sfPlayer.Player.armor.Any(item => item.type == ModContent.ItemType<ReverseCharm>()))
                return false;

            if (sfPlayer.blackFlashCounter < 2)
                return false;

            if (sfPlayer.TryGetQuestData(this, "HealthPotionsConsumed", out object countData))
            {
                int count = (int)countData;
                if (count < HEALTH_POTIONS)
                    return false;
            }
            else
                return false;

            return true;
        }

        public override void UsedItem(SorceryFightPlayer sfPlayer, Item item)
        {
            if (item.healLife > 0)
            {
                if (sfPlayer.TryGetQuestData(this, "HealthPotionsConsumed", out object countData))
                {
                    int count = (int)countData;
                    count++;
                    sfPlayer.ModifyQuestData(this, "HealthPotionsConsumed", count);
                }
                else
                {
                    sfPlayer.ModifyQuestData(this, "HealthPotionsConsumed", 1);
                }

                Main.NewText(sfPlayer.GetQuestData(this, "HealthPotionsConsumed"));
            }
        }
    }
}