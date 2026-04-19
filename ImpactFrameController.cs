using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class ImpactFrameController
    {
        private static int timeLeft;
        public static void ImpactFrame(Color impactFrameColor, int duration)
        {
            if (Main.dedServ) return;
            if (ModContent.GetInstance<ClientConfig>().DisableImpactFrames) return;

            if (!Filters.Scene["SF:ImpactFrame"].IsActive())
            {
                Filters.Scene.Activate("SF:ImpactFrame").GetShader().UseProgress(0).UseOpacity(1);
                timeLeft = 0;
            }

            TaskScheduler.Instance.AddContinuousTask(() =>
            {
                int dur = duration;
                ref int tL = ref timeLeft;

                float progress = (float)tL / dur;
                if (progress > 0.5)
                    Filters.Scene["SF:ImpactFrame"].GetShader().UseProgress(1).UseColor(impactFrameColor);

                tL++;
                
            }, duration);

            TaskScheduler.Instance.AddDelayedTask(() =>
            {
                if (Filters.Scene["SF:ImpactFrame"].IsActive())
                {
                    Filters.Scene["SF:ImpactFrame"].GetShader().UseProgress(0).UseOpacity(0);
                    Filters.Scene.Deactivate("SF:ImpactFrame");
                }
            }, duration + 1);
        }
    }
}
