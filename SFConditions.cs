using System;
using System.Collections.Generic;
using System.Linq;
using sorceryFight.SFPlayer;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{

    public static class SFConditions
    {
        public class FirstTimeDefeatingBoss : IItemDropRuleCondition, IProvideItemConditionDescription
        {
            internal class BossBagTracker : ModPlayer
            {
                internal List<int> bossBags;
                public override void Initialize()
                {
                    bossBags = new List<int>();
                }

                public override void SaveData(TagCompound tag)
                {
                    tag["bossBags"] = bossBags;
                }

                public override void LoadData(TagCompound tag)
                {
                    bossBags = tag.ContainsKey("bossBags") ? tag.GetList<int>("bossBags").ToList() : new List<int>();
                }

                internal void AddBossBag(int itemID)
                {
                    bossBags.Add(itemID);
                }

                internal bool HasBossBag(int itemID)
                {
                    return bossBags.Contains(itemID);
                }
            }


            public bool CanDrop(DropAttemptInfo info)
            {
                if (info.npc == null)
                {
                    if (!info.player.GetModPlayer<BossBagTracker>().HasBossBag(info.item))
                    {
                        info.player.GetModPlayer<BossBagTracker>().AddBossBag(info.item);
                        return true;
                    }
                    else
                        return false;
                }
                else
                    return !info.player.GetModPlayer<SorceryFightPlayer>().HasDefeatedBoss(info.npc.type);

            }

            public bool CanShowItemDropInUI()
            {
                return true;
            }

            public string GetConditionDescription()
            {
                return "not implemented";
            }
        }
    }
}
