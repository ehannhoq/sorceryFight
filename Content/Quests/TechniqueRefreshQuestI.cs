using sorceryFight.SFPlayer;

namespace sorceryFight.Content.Quests
{
    public class TechniqueRefreshQuestI : Quest
    {
        public override bool IsCompleted(SorceryFightPlayer sfPlayer)
        {
            return false;
        }

        public override void GiveRewards(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.sixEyesLevel = 2;
        }
    }
}