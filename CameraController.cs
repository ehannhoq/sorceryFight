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
        private static bool cameraPositionActive;
        private static Vector2 originalCameraZoom;
        private static Vector2 targetCameraZoom;
        private static bool cameraZoomActive;

        private static int shakeTimeLeft = 0;

        public static void SetCameraPosition(Vector2 position, int duration, float lerp = 0.5f)
        {
            if (Main.dedServ) return;
            
            if (!cameraPositionActive)
            {
                cameraPositionActive = true;
                originalCameraPosition = Main.screenPosition;
                targetCameraPosition = originalCameraPosition;
            }

            Action lerpCamera = () =>
            {
                targetCameraPosition = Vector2.Lerp(
                    targetCameraPosition,
                    position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2),
                    lerp
                );
            };

            TaskScheduler.Instance.AddContinuousTask(lerpCamera, duration);
            TaskScheduler.Instance.AddDelayedTask(ResetCameraPosition, duration + 1);
        }

        public static void SetCameraPosition(Vector2 position, float lerp = 0.5f)
        {
            if (Main.dedServ) return;
            
            if (!cameraPositionActive)
            {
                cameraPositionActive = true;
                originalCameraPosition = Main.screenPosition;
                targetCameraPosition = originalCameraPosition;
            }

            targetCameraPosition = Vector2.Lerp(
                targetCameraPosition,
                position - new Vector2(Main.screenWidth / 2, Main.screenHeight / 2),
                lerp
            );
        }

        public static void SetCameraZoom(Vector2 zoom, int duration, float lerp = 0.5f)
        {
            if (Main.dedServ) return;

            if (!cameraZoomActive)
            {
                cameraZoomActive = true;
                originalCameraZoom = Main.BackgroundViewMatrix.Zoom;
                targetCameraZoom = originalCameraZoom;
            }

            Action lerpZoom = () =>
            {
                targetCameraZoom = Vector2.Lerp(
                    targetCameraZoom,
                    zoom,
                    lerp
                );
            };

            TaskScheduler.Instance.AddContinuousTask(lerpZoom, duration);
            TaskScheduler.Instance.AddDelayedTask(ResetCameraZoom, duration + 1);
        }

        public static void SetCameraZoom(Vector2 zoom, float lerp = 0.5f)
        {
            if (Main.dedServ) return;

            if (!cameraZoomActive)
            {
                cameraZoomActive = true;
                originalCameraZoom = Main.BackgroundViewMatrix.Zoom;
                targetCameraZoom = originalCameraZoom;
            }

            targetCameraZoom = Vector2.Lerp(
                targetCameraZoom,
                zoom,
                lerp
            );
        }

        public static void CameraShake(int duration, float xShake = 0, float yShake = 0)
        {
            if (Main.dedServ) return;
            
            if (!cameraPositionActive)
            {
                cameraPositionActive = true;
                originalCameraPosition = Main.screenPosition;
                targetCameraPosition = originalCameraPosition;
            }

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
                ResetCameraPosition();
                shakeTimeLeft = 0;
            }, duration + 1);
        }

        public static void ResetCameraPosition()
        {
            if (Main.dedServ) return;

            cameraPositionActive = false;
            Main.screenPosition = originalCameraPosition;
        }

        public static void ResetCameraZoom()
        {
            if (Main.dedServ) return;

            cameraZoomActive = false;
            Main.BackgroundViewMatrix.Zoom = originalCameraZoom;
        }

        public static void ResetCamera()
        {
            ResetCameraPosition();
            ResetCameraZoom();
        }

        public override void ModifyScreenPosition()
        {
            if (cameraPositionActive)
                Main.screenPosition = targetCameraPosition;
        }

        public override void ModifyTransformMatrix(ref SpriteViewMatrix Transform)
        {
            if (cameraZoomActive)
                Transform.Zoom = targetCameraZoom;
        }
    }
}