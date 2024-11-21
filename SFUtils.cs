using System;
using System.Collections.Generic;
using sorceryFight.Content.DomainExpansions;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight;

public static class SFUtils
{
    /// <summary>
    /// THANK YOU CALAMITY MOD SOURCE CODE FOR THIS !!
    /// 
    /// Adds to a list on a given condition.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="type"></param>
    /// <param name="condition"></param>
    public static void AddWithCondition<T>(this List<T> list, T type, bool condition)
    {
        if (condition)
            list.Add(type);
    }

    public static bool IsDomain (this NPC npc)
    {
        return npc.type == ModContent.NPCType<UnlimitedVoid>();
    }
    
    public static LocalizedText GetLocalization(string key)
    {
        return Language.GetText(key);
    }

    public static string GetLocalizationValue(string key)
    {
        return Language.GetTextValue(key);
    }
}
