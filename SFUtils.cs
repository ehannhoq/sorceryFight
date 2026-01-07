using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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

    public static List<string> GetLocalizationValues(string key)
    {
        return Language.GetTextValue(key).Split('\n').ToList();
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

    /// <summary>
    /// Clamps each element of the given array to the given minimum and maximum values.
    /// </summary>
    /// <param name="ints">The array to clamp.</param>
    /// <param name="min">The minimum value to clamp to.</param>
    /// <param name="max">The maximum value to clamp to.</param>
    public static void Clamp(this int[] ints, int min, int max)
    {
        for (int i = 0; i < ints.Length; i++)
        {
            ints[i] = Math.Clamp(ints[i], min, max);
        }
    }

    /// <summary>
    /// Gets a value from an internal field in a Calamity type. This is used to access fields that are not publicly exposed.
    /// 
    /// developer's note: i love calamity but fuck you for whoever thought of the shit ass way to modify vanilla boss -e
    /// </summary>
    /// <typeparam name="T">The type of the value to get.</typeparam>
    /// <param name="typeName">The name of the Calamity type to access. This should be the full name of the type, without the "CalamityMod" namespace.</param>
    /// <param name="fieldName">The name of the field to access.</param>
    /// <param name="instance">The instance of the type to access the field from. If null, accesses a static field.</param>
    /// <returns>The value of the field.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the type or field could not be found.</exception>
    public static T GetInternalFieldFromCalamity<T>(string typeName, string fieldName, object instance = null)
    {
        var t = Type.GetType(typeName + ", CalamityMod");
        if (t == null)
            throw new InvalidOperationException($"Could not find Calamity type {typeName}.");

        var f = t.GetField(fieldName,
                    BindingFlags.NonPublic | BindingFlags.Public |
                    BindingFlags.Static | BindingFlags.Instance);

        if (f == null)
            throw new InvalidOperationException($"Could not find field {fieldName} on {typeName}.");

        return (T)f.GetValue(instance);
    }

    public static int Append<T>(this T[] array, T obj)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == null)
            {
                array[i] = obj;
                return i;
            }
        }

        return -1;
    }

    public static int Prepend<T>(this T[] array, T obj)
    {
        for (int i = array.Length - 1; i >= 0; i--)
        {
            if (array[i] == null)
            {
                array[i] = obj;
                return i;
            }
        }

        return -1;
    }

    public static int FindIndex<T>(this T[] array, Predicate<T> predicate)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (predicate(array[i]))
                return i;
        }

        return -1;
    }

    public static bool TryGet<T>(this IEnumerable<T> array, Predicate<T> predicate, [MaybeNullWhen(false)] out T obj)
    {
        obj = default;
        for (int i = 0; i < array.Count(); i++)
        {
            if (array.ElementAt(i) == null) continue;

            if (predicate(array.ElementAt(i)))
            {
                obj = array.ElementAt(i);
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Copied from CalamityMod's PlayerUtils.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="needsToHold"></param>
    /// <returns>Whether if the player is dead, inactive, or otherwise unable to use holdout items.</returns>
    public static bool CantUseHoldout(this Player player, bool needsToHold = true) => player == null || !player.active || player.dead || (!player.channel && needsToHold) || player.CCed || player.noItems;

    public static bool CantUseSword(this Player player, Projectile slash, bool needsToHold = true)
    {
        return player.CantUseHoldout(needsToHold) || player.HeldItem.shoot != slash.type;
    }


    public static Type FindTypeAcrossMods(string fullName)
    {
        Type type = Type.GetType(fullName);
        if (type != null)
            return type;

        foreach (var mod in ModLoader.Mods)
        {
            var modType = mod.Code.GetType(fullName);
            if (modType != null)
                return modType;
        }

        return null;
    }


    /// <summary>
    /// Helper method that calculates and positions the projectile if the projectile is serving as a slash for a weapon.
    /// </summary>
    /// <param name="proj"></param>
    public static void PositionProjectileForSlash(this Projectile proj, float offset)
    {
        Player player = Main.player[proj.owner];
        Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
        float velocityAngle = proj.velocity.ToRotation();

        proj.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
        proj.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();
        proj.rotation = velocityAngle + (proj.direction == -1).ToInt() * MathHelper.Pi;
        proj.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
        player.ChangeDir(proj.direction);

        if (Main.myPlayer == proj.owner)
        {

            if (player.CantUseSword(proj))
            {
                proj.Kill();
            }
        }
    }
}

public static class SFConstants
{
    public const int SixEyesPercent = 10;
    public const int UniqueBodyStructurePercent = 15;
    public const int BlessedByBlackSparksPercent = 10;
    public const int ExplosiveCursedEnergyPercent = 15;
    public const int SharpCursedEnergyPercent = 15;
    public const int OverflowingEnergyPercent = 15;
}
