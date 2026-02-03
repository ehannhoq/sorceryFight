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
    public class RCTQuestI : Quest
    {
        private const int HP_TO_HEAL = 200;
        public override bool IsCompleted(SorceryFightPlayer sfPlayer)
        {
            if (sfPlayer.rctAuraIndex != -1)
            {
                if (sfPlayer.TryGetQuestData(this, "HealthRegeneratedWithRCT", out object countData))
                {
                    float currentHealed = (float)countData;
                    sfPlayer.ModifyQuestData(this, "HealthRegeneratedWithRCT", currentHealed + SFUtils.RateSecondsToTicks(sfPlayer.rctBaseHealPerSecond + sfPlayer.additionalRCTHealPerSecond));

                    if (currentHealed > HP_TO_HEAL)
                        return true;
                }
                else
                {
                    sfPlayer.ModifyQuestData(this, "HealthRegeneratedWithRCT", SFUtils.RateSecondsToTicks(sfPlayer.rctBaseHealPerSecond + sfPlayer.additionalRCTHealPerSecond));
                }
            }

            return false;
        }

        public override void GiveRewards(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.Player.QuickSpawnItem(sfPlayer.Player.GetSource_Misc("ReverseCharm"), ModContent.ItemType<ReverseCharm>(), 1);
        }
    }
}