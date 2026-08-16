using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight
{
    public static partial class SFUtils
    {
        public static LocalizedText GetLocalization(string key)
        {
            return Language.GetText(key);
        }

        public static string GetLocalizationValue(string key)
        {
            return Language.GetTextValue(key);
        }

        public static List<string> GetLocalizationValues(string key)
        {
            return Language.GetTextValue(key).Split('\n').ToList();
        }

        public static string GetUnlockRequirementFromBossID(int type)
        {
            if (type == -1)
                return $"Boss type incorrectly set";
                
            ModNPC modBoss = NPCLoader.GetNPC(type);
            string npcName = modBoss != null ? modBoss.DisplayName.Value : Lang.GetNPCNameValue(type);

            npcName = npcName.Replace(" ", "");
            npcName = npcName.Replace("'", "");

            return GetLocalizationValue($"Mods.sorceryFight.UnlockRequirements.{npcName}");
        }

        public static void FindAndReplace(this List<TooltipLine> tooltips, string value, string newValue)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Text.Contains(value));
            if (line != null)
                line.Text = line.Text.Replace(value, newValue);
        }
    }
}