using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.Buffs.Shrine;
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

    public static bool IsDomain(this NPC npc)
    {   
        return npc.type == ModContent.NPCType<UnlimitedVoid>() || npc.type == ModContent.NPCType<MalevolentShrine>();   
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

        if (proj.type == ModContent.ProjectileType<DomainAmplificationProjectile>())
            return false;
        
        if (proj.type == ModContent.ProjectileType<HollowWickerBasketProjectile>())
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

    /// <summary>
    /// Draws a line between two points using a sprite batch. TAKEN FROM CALAMITY MOD, MODIFIED BY EHANN
    /// </summary>
    /// <param name="spriteBatch">The sprite batch to draw with.</param>
    /// <param name="start">The starting point of the line.</param>
    /// <param name="end">The ending point of the line.</param>
    /// <param name="color">The color to draw the line with.</param>
    /// <param name="width">The width of the line.</param>
    public static void DrawLineUI(this SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color, float width)
    {
        if (Main.dedServ) return;
        if (start == end)
            return;

        Texture2D line = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Line").Value;
        float rotation = (end - start).ToRotation();
        Vector2 scale = new Vector2(Vector2.Distance(start, end) / line.Width, width);

        spriteBatch.Draw(line, start, null, color, rotation, line.Size() * Vector2.UnitY * 0.5f, scale, SpriteEffects.None, 0f);
    }

    /// <param name="denominator">The chance denominator for the roll</param>
    /// <returns>True if a a 1 in denominator is rolled, false otherwise</returns>
    public static bool Roll(int denominator)
    {
        int roll = Main.rand.Next(0, denominator);

        if (roll == 0) return true;
        return false;
    }
    
    /// <summary>
    /// Returns the square of the input value.
    /// </summary>
    /// <param name="value">The value to square.</param>
    /// <returns>The square of the input value.</returns>
    public static float Squared(this float value)
    {
        return value * value;
    }
}

public static class SFConstants
{
    public const int SixEyesDenominator = 10;
    public const int UniqueBodyStructureDenominator = 15;

}
