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

        public override void Load()
        {
            Main.GameViewMatrix.Zoom = new Vector2(1, 1);
        }

        public static void SetCameraPosition(Vector2 position, int duration, float lerp = 0.5f)
        {
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

        public static void ResetCameraZoom()
        {
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