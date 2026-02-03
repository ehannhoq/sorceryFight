using System;
using System.Reflection;

namespace sorceryFight.Content.UI.Dialog.Actions
{
    public class EndOfDialogAction : IAction
    {
        public string methodName;
        private object initiator;
        

        public EndOfDialogAction(string methodName)
        {
            this.methodName = methodName;
        }


        public void Invoke()
        {
            if (initiator == null) return;

            var method = initiator.GetType().GetMethod(methodName,
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                ?? throw new Exception($"Method {methodName} not found in {initiator.GetType().Name}");

            method.Invoke(initiator, null);
        }


        public void SetInitiator(object initiator)
        {
            this.initiator = initiator;
        }


        public string GetUIText()
        {
            return "";
        }
    }
}