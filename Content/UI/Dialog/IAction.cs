namespace sorceryFight.Content.UI.Dialog
{
    public interface IAction 
    {
        public void Invoke();
        public string GetUIText();
        public void SetInitiator(object initiator);
    }
}