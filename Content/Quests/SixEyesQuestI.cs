using sorceryFight.Content.Items.Accessories;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Quests
{
    public class SixEyesQuestI : Quest
    {
        private const int NPC_KILL_COUNT = 5;
        public override void OnAddedQuest(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.Player.QuickSpawnItem(sfPlayer.Player.GetSource_Misc("PitchBlack_Blindfold"), ModContent.ItemType<CursedBlindfold>());
        }

        public override bool IsAvailable(SorceryFightPlayer sfPlayer)
        {
            return sfPlayer.AwakenedSixEyes;
        }

        public override bool IsCompleted(SorceryFightPlayer sfPlayer)
        {
            if (sfPlayer.TryGetQuestData(this, "NPCKillCount", out object data))
            {
                int currentCount = (int)data;
                if (currentCount >= NPC_KILL_COUNT)
                    return true;
            }

            return false;
        }

        public override void KilledNPC(SorceryFightPlayer sfPlayer, NPC npc)
        {
            if (!sfPlayer.cursedBlindfold) return;

            if (sfPlayer.TryGetQuestData(this, "NPCKillCount", out object data))
            {
                int currentCount = (int)data;
                sfPlayer.ModifyQuestData(this, "NPCKillCount", currentCount + 1);
            }
            else
            {
                sfPlayer.ModifyQuestData(this, "NPCKillCount", 1);
            }
        }

        public override void GiveRewards(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.sixEyesLevel = 2;
        }
    }
}