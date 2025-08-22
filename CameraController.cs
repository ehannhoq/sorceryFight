using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class CameraController : ModSystem
    {
        private static Vector2 originalCameraPosition;
        private static Vector2 targetCameraPosition;
        private static Vector2 originalCameraZoom;
        private static Vector2 targetCameraZoom;
        private static int shakeTimeLeft = 0;

        public static void SetCameraPosition(Vector2 position, int duration, float lerp = 0.5f)
        {
            if (Main.dedServ) return;

            Action action = () =>
            {
                if (originalCameraPosition == Vector2.Zero)
                    originalCameraPosition = Main.screenPosition;

                if (targetCameraPosition == Vector2.Zero)
                    targetCameraPosition = Main.screenPosition;

                targetCameraPosition = Vector2.Lerp(
                    targetCameraPosition,
                    position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2),
                    lerp
                );

                Main.screenPosition = position;
            };

            TaskScheduler.Instance.AddContinuousTask(action, duration);
            TaskScheduler.Instance.AddDelayedTask(() =>
            {
                Main.screenPosition = originalCameraPosition;
                originalCameraPosition = Vector2.Zero;
                targetCameraPosition = Vector2.Zero;
            }, duration + 1);
        }

        public static void SetCameraZoom(Vector2 zoom, int duration = -1, float lerp = 0.5f)
        {
            if (Main.dedServ) return;

            Action action = () =>
            {
                if (originalCameraZoom == Vector2.Zero)
                    originalCameraZoom = Main.BackgroundViewMatrix.Zoom;

                if (targetCameraZoom == Vector2.Zero)
                    targetCameraZoom = Main.BackgroundViewMatrix.Zoom;

                targetCameraZoom = Vector2.Lerp(
                    targetCameraZoom,
                    zoom,
                    lerp
                );
            };

            if (duration == -1)
                action();
            else
            {
                TaskScheduler.Instance.AddContinuousTask(action, duration);
                TaskScheduler.Instance.AddDelayedTask(() =>
                {
                    Main.BackgroundViewMatrix.Zoom = originalCameraZoom;
                    originalCameraZoom = Vector2.Zero;
                    targetCameraZoom = Vector2.Zero;
                }, duration + 1);
            }
        }

        public static void CameraShake(int duration, float xShake = 0, float yShake = 0)
        {
            if (Main.dedServ) return;

            if (originalCameraPosition == Vector2.Zero)
                originalCameraPosition = Main.screenPosition;

            if (targetCameraPosition == Vector2.Zero)
                targetCameraPosition = Main.screenPosition;

            targetCameraPosition += new Vector2(Main.rand.NextFloat(-xShake, xShake), Main.rand.NextFloat(-yShake, yShake));

            TaskScheduler.Instance.AddContinuousTask(() =>
            {
                Vector2 playerCameraPosition = Main.LocalPlayer.Center - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

                float xVariation = xShake;
                float yVariation = yShake;
                ref int timeLeft = ref shakeTimeLeft;

                float progress = (float)timeLeft / duration;
                float currentShakeX = MathHelper.Lerp(xVariation, 0, progress);
                float currentShakeY = MathHelper.Lerp(yVariation, 0, progress);

                targetCameraPosition = playerCameraPosition + new Vector2(
                    Main.rand.NextFloat(-currentShakeX, currentShakeX),
                    Main.rand.NextFloat(-currentShakeY, currentShakeY)
                );

                timeLeft++;

            }, duration);

            TaskScheduler.Instance.AddDelayedTask(() =>
            {
                Main.screenPosition = originalCameraPosition;
                originalCameraPosition = Vector2.Zero;
                targetCameraPosition = Vector2.Zero;
                shakeTimeLeft = 0;
            }, duration + 1);
        }

        public static void ResetCameraZoom()
        {
            if (Main.dedServ) return;
            if (originalCameraZoom == Vector2.Zero) return;

            Main.BackgroundViewMatrix.Zoom = originalCameraZoom;
            originalCameraZoom = Vector2.Zero;
            targetCameraZoom = Vector2.Zero;
        }

        public override void ModifyScreenPosition()
        {
            if (targetCameraPosition != Vector2.Zero)
                Main.screenPosition = targetCameraPosition;
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (targetCameraZoom != Vector2.Zero)
                Transform.Zoom = targetCameraZoom;
        }
    }

}