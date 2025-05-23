using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Particles.UIParticles
{
    public class ParticleController : ModSystem
    {
        private static List<Particle> particles;


        public override void Load()
        {
            particles = new List<Particle>();
            IL_Main.DoDraw_DrawNPCsOverTiles += DrawUIParticleLayer;
        }

        public override void Unload()
        {
            particles = null;
            IL_Main.DoDraw_DrawNPCsOverTiles -= DrawUIParticleLayer;
        }

        private void DrawUIParticleLayer(ILContext il)
        {
            if (Main.dedServ) return;
            var cursor = new ILCursor(il);

            cursor.Goto(1);

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

                foreach (var particle in new List<Particle>(particles))
                {
                    if (particle == null) continue;

                    particle.Draw(Main.spriteBatch);
                }

                Main.spriteBatch.End();
            });
        }

        public override void PostUpdateNPCs()
        {
            foreach (var particle in new List<Particle>(particles))
            {
                if (particle == null) continue;

                particle.Update();

                if (particle.time >= particle.lifetime)
                {
                    particles.Remove(particle);
                }
            }
        }

        public static void SpawnParticleInWorld(Particle particle)
        {
            if (Main.dedServ) return;

            particle.position -= Main.screenPosition;
            particles.Add(particle);
        }

        public static void SpawnParticleInUI(Particle particle)
        {
            if (Main.dedServ) return;

            particles.Add(particle);
        }
    }
}