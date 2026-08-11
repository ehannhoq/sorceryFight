using System;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace sorceryFight.Utilities.EaseFunctions
{
    public static class EaseFunctions
    {
        /// <summary>
        /// Performs an EaseInOut function on a given normalized input.
        /// </summary>
        public static float EaseInOut(float x)
        {
            if (x < 0.0f) return 0.0f;
            if (x > 1.0f) return 1.0f;

            return (0.5f * -MathF.Cos(-MathHelper.Pi * x)) + 0.5f;
        }

        /// <summary>
        /// Performs an EaseIn function on a given normalized input.
        /// </summary>
        public static float EaseIn(float x)
        {
            if (x < 0.0f) return 1.0f;
            if (x > 1.0f) return 0.0f;

            return -MathF.Cos(-MathHelper.PiOver2 * x) + 1;
        }


        /// <summary>
        /// Performs an EaseOut function on a given normalized input.
        /// </summary>
        public static float EaseOut(float x)
        {
            if (x < 0.0f) return 0.0f;
            if (x > 1.0f) return 1.0f;

            return -MathF.Cos(-MathHelper.PiOver2 * x) + 1;
        }


        /// <summary>
        /// Performs an EaseInOut function on a given normalized input, but:
        /// <para>
        /// f(1.0) = f(1.0) = 0.0
        /// </para>
        /// <para>
        /// f(0.5) = 1.0
        /// </para>
        /// </summary>
        public static float EaseInOutZero(float x)
        {
            if (x < 0.0f) return 0.0f;
            if (x > 1.0f) return 0.0f;

            return -0.5f * MathF.Cos(-MathHelper.TwoPi * x) + 0.5f;
        }

        public static float EaseInExponential(float power, float x)
        {
            if (x < 0.0f) return 0.0f;
            if (x > 1.0f) return 1.0f;
            return MathF.Pow(x, power);
        }


        public static float EaseOutExponential(float power, float x)
        {
            if (x < 0.0f) return 0.0f;
            if (x > 1.0f) return 1.0f;

            return MathF.Pow(x - 1, power) + (power % 2 == 0 ? 0 : 1);
        }

        public static float EaseInCircular(float x)
        {
            if (x < 0.0f) return 0.0f;
            if (x > 1.0f) return 1.0f;

            return -MathF.Sqrt(1 - x * x) + 1;
        }

        public static float EaseOutCircular(float x)
        {
            if (x < 0.0f) return 0.0f;
            if (x > 1.0f) return 1.0f;

            return MathF.Sqrt(1 - MathF.Pow(x - 1, 2));
        }
    }
}
