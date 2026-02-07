using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Particles.UIParticles
{
    public class ParticleController : ModSystem
    {
        private static Particle[] particles;


        public override void Load()
        {
            particles = new Particle[5000];
            IL_Main.DoDraw_DrawNPCsOverTiles += DrawUIParticleLayer;
        }

        public override void Unload()
        {
            particles = null;
            IL_Main.DoDraw_DrawNPCsOverTiles -= DrawUIParticleLayer;
        }

        public override void OnWorldUnload()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = null;
            }
        }

        private void DrawUIParticleLayer(ILContext il)
        {
            if (Main.dedServ) return;
            var cursor = new ILCursor(il);

            cursor.Goto(-1);

            cursor.EmitDelegate(() =>
            {
                Main.spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    null,
                    Main.GameViewMatrix.ZoomMatrix
                );

                for (int i = 0; i < particles.Length; i++)
                {
                    if (particles[i] == null) continue;

                    particles[i].Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();
            });
        }

        public override void PostUpdateNPCs()
        {
            if (Main.GameUpdateCount % 3 == 1) return;

            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i] == null) continue;

                particles[i].Update();

                if (particles[i].time >= particles[i].lifetime)
                {
                    particles[i] = null;
                }
            }
        }

        public static void SpawnParticle(Particle particle)
        {
            if (Main.dedServ || Main.gamePaused || particles == null) return;
                particles.Append(particle);
        }
    }
}