using System;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class CameraController : ModSystem
    {
        private static Vector2 targetScreenPosition;
        private static float screenPositionInterlopant;

        private static float zoomMultiplier;

        public static void SetCameraPosition(Vector2 worldPos, int duration = 0, float lerp = 0.5f)
        {
            if (Main.dedServ) return;

            targetScreenPosition = worldPos;
            screenPositionInterlopant = lerp;

            if (duration > 0)
                TaskScheduler.Instance.AddDelayedTask(ResetCameraPosition, duration);
        }

        public static void SetCameraZoom(float zoomMult, int duration = 0, float lerp = 0.5f)
        {
            if (Main.dedServ) return;

            zoomMultiplier = MathHelper.Lerp(
                zoomMultiplier,
                zoomMult,
                lerp
            );

            if (duration > 0)
                TaskScheduler.Instance.AddDelayedTask(ResetCameraZoom, duration);
        }

        public static void CameraShake(int duration, float xShake, float yShake)
        {
            screenPositionInterlopant = 0.8f;

            TaskScheduler.Instance.AddContinuousTask(() =>
            {
                float x = xShake;
                float y = yShake;

                targetScreenPosition = Main.LocalPlayer.Center + Main.rand.NextVector2CircularEdge(xShake, yShake);

            }, duration);

            TaskScheduler.Instance.AddDelayedTask(ResetCameraPosition, duration + 1);
        }


        public static void ResetCameraPosition()
        {
            if (Main.dedServ) return;

            TaskScheduler.Instance.AddContinuousTask(() =>
            {
                targetScreenPosition = Vector2.Lerp(targetScreenPosition, Main.LocalPlayer.Center, screenPositionInterlopant);
            }, 60);

            TaskScheduler.Instance.AddDelayedTask(() =>
            {
                targetScreenPosition = Main.LocalPlayer.Center;
                screenPositionInterlopant = 0f;
            }, 61);
        }

        public static void ResetCameraZoom()
        {
            if (Main.dedServ) return;

            TaskScheduler.Instance.AddContinuousTask(() =>
            {
                zoomMultiplier = MathHelper.Lerp(zoomMultiplier, 0f, 0.5f);
            }, 60);

            TaskScheduler.Instance.AddDelayedTask(() =>
            {
                zoomMultiplier = 0f;
            }, 61);
        }

        public override void ModifyScreenPosition()
        {
            if (Main.LocalPlayer.dead || !Main.LocalPlayer.active)
            {
                ResetCameraPosition();
                ResetCameraZoom();
                return;
            }

            Vector2 idealScreenPosition = targetScreenPosition - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
            Main.screenPosition = Vector2.Lerp(Main.screenPosition, idealScreenPosition, screenPositionInterlopant);
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            zoomMultiplier = Math.Clamp(zoomMultiplier, -0.9f, 5f);
            Transform.Zoom *= 1 + zoomMultiplier;
        }
    }
}