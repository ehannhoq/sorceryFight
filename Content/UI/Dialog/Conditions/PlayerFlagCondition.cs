using System;
using System.Reflection;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;

namespace sorceryFight.Content.UI.Dialog.Conditions
{
    public class PlayerFlagCondition : ICondition
    {
        private string flag;
        private string value;

        public PlayerFlagCondition(string flag, string value)
        {
            this.flag = flag;
            this.value = value;
        }
        
        public bool Evaluate(SorceryFightPlayer sfPlayer)
        {
            Type type = sfPlayer.GetType();

            PropertyInfo prop = type.GetProperty(flag, BindingFlags.Instance | BindingFlags.Public);
            if (prop != null)
            {
                return Compare(prop.GetValue(sfPlayer), prop.PropertyType);
            }
            ModContent.GetInstance<SorceryFight>().Logger.Debug("Content/UI/Dialog/Conditions/FlagCondition: 'prop' was null.");

            FieldInfo field = type.GetField(flag, BindingFlags.Instance | BindingFlags.Public);
            if (field != null)
            {
                return Compare(field.GetValue(sfPlayer), field.FieldType);
            }

            ModContent.GetInstance<SorceryFight>().Logger.Debug("Content/UI/Dialog/Conditions/FlagCondition: 'field' was null.");
            return false;
        }

        private bool Compare(object currentValue, Type targetType)
        {
            if (currentValue == null)
                return false;

            try
            {
                object converted;

                if (targetType.IsEnum)
                {
                    converted = Enum.Parse(targetType, value, ignoreCase: true);
                }
                else
                {
                    converted = Convert.ChangeType(value, targetType);
                }

                return currentValue.Equals(converted);
            }
            catch
            {
                ModContent.GetInstance<SorceryFight>().Logger.Debug("Content/UI/Dialog/Conditions/FlagCondition: something went wrong with conversion.");
                return false;
            }
        }
    }
}