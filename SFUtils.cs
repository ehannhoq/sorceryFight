using System;
using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.DomainExpansions;
using Terraria;
using Terraria.ID;
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

    public static bool MoveableByBlue(this NPC npc)
    {
        if (npc.type == NPCID.DD2LanePortal)
            return false;

        return true;
    }

    public static bool MoveableByBlue(this Projectile proj)
    {
        if (proj.type == ModContent.ProjectileType<AmplifiedAuraProjectile>())
            return false;

        if (proj.type == ModContent.ProjectileType<MaximumAmplifiedAuraProjectile>())
            return false;

        if (proj.type == ModContent.ProjectileType<ReverseCursedTechniqueAuraProjectile>())
            return false;

        return true;
    }
    
    public static LocalizedText GetLocalization(string key)
    {
        return Language.GetText(key);
    }

    public static string GetLocalizationValue(string key)
    {
        return Language.GetTextValue(key);
    }

    public static NetworkText GetNetworkText(string key)
    {
        return NetworkText.FromKey(key);
    }
}
