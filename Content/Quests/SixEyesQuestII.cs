using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.Content.Buffs;
using Terraria.ID;

namespace sorceryFight.Content.Quests
{
    public class SixEyesQuestII : Quest
    {
        public override bool IsAvailable(SorceryFightPlayer sfPlayer)
        {
            return sfPlayer.SixEyes;
        }

        public override bool IsCompleted(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.Player.AddBuff(ModContent.BuffType<StrainedSixEyes>(), 2);

            if (sfPlayer.TryGetQuestData(this, "KilledLunaticCultist", out object data))
            {
                bool killedBoss = (bool)data;
                if (killedBoss)
                    return true;
            }

            return false;
        }

        public override void KilledNPC(SorceryFightPlayer sfPlayer, NPC npc)
        {
            if (npc.type != NPCID.CultistBoss) return;

            sfPlayer.ModifyQuestData(this, "KilledLunaticCultist", true);
        }

        public override void GiveRewards(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.sixEyesLevel = 3;
        }
    }
}