using System;
using System.Reflection;

namespace sorceryFight.Content.UI.Dialog
{
    public class InvokeMethodAction : IAction
    {
        public string methodName;
        public string uiText;
        private object initiator;
        

        public InvokeMethodAction(string methodName, string uiText)
        {
            this.methodName = methodName;
            this.uiText = uiText;
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
            return uiText;
        }
    }
}