using sorceryFight.SFPlayer;

namespace sorceryFight.Content.UI.Dialog.Conditions
{
    public interface ICondition
    {
        public bool Evaluate(SorceryFightPlayer sfPlayer);
    }
}