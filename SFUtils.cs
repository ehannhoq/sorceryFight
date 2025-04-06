using System.Collections.Generic;
using CalamityMod.NPCs.DevourerofGods;
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

    /// <summary>
    /// Converts seconds into buff time.
    /// </summary>
    /// <param name="seconds"></param>
    /// <returns>The number of ticks in a second.</returns>
    public static int BuffSecondsToTicks(float seconds)
    {
        return (int)(seconds * 60);
    }

    /// <summary>
    /// Converts x/second into x/ticks. Usually used for CE regen and CE consumption.
    /// </summary>
    /// <param name="ticks"></param>
    /// <returns>The rate per tick.</returns>
    public static float RateSecondsToTicks(float ticks)
    {
        return ticks / 60;
    }

    public static bool IsDomain(this NPC npc)
    {
        return npc.type == ModContent.NPCType<UnlimitedVoid>() || npc.type == ModContent.NPCType<MalevolentShrine>() || npc.type == ModContent.NPCType<Home>();
    }

    public static bool MoveableByBlue(this NPC npc)
    {
        if (npc.type == NPCID.DD2LanePortal)
            return false;

        if (npc.type == ModContent.NPCType<DevourerofGodsBody>() || npc.type == ModContent.NPCType<DevourerofGodsHead>() || npc.type == ModContent.NPCType<DevourerofGodsTail>())
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


    /// <summary>
    /// Returns true if a random number between 0 and 99 is less than <paramref name="percentChance"/>, false otherwise.
    /// </summary>
    /// <param name="percentChance">The percentage chance of returning true.</param>
    public static bool Roll(int percentChance)
    {
        int roll = Main.rand.Next(0, 100);
        return roll < percentChance;
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

    /// <summary>
    /// Determines the sign of a floating-point value.
    /// </summary>
    /// <param name="value">The floating-point value to evaluate.</param>
    /// <returns>1 if the value is positive, -1 if negative, or 0 if zero.</returns>

    public static float ToSign(this float value)
    {
        return value > 0 ? 1 : (value < 0 ? -1 : 0);
    }

    /// <summary>
    /// Linearly interpolates between two angles by a given amount, taking
    /// into account the wraparound of angles from 0 to 2 * pi.
    /// </summary>
    /// <param name="currentAngle">The current angle.</param>
    /// <param name="targetAngle">The target angle to interpolate to.</param>
    /// <param name="amount">The amount to interpolate by, from 0 to 1.</param>
    /// <returns>The interpolated angle.</returns>
    public static float LerpAngle(float currentAngle, float targetAngle, float amount)
    {
        float difference = MathHelper.WrapAngle(targetAngle - currentAngle);
        return currentAngle + difference * amount;
    }

}

public static class SFConstants
{
    public const int SixEyesPercent = 10;
    public const int UniqueBodyStructurePercent = 15;
}
