using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Particles.UIParticles
{
    public class ParticleController : ModSystem
    {

        //ParticleSF and ParticleController are Ehann's particle designs
        //Particle and GeneralParticleHandler are Calamity's

        private static ParticleSF[] particles;
        private static int firstFreeIndex;

        public override void Load()
        {
            particles = new ParticleSF[1000];
            IL_Main.DoDraw_DrawNPCsOverTiles += DrawUIParticleLayer;
            firstFreeIndex = 0;

        }

        public override void Unload()
        {
            particles = null;
            IL_Main.DoDraw_DrawNPCsOverTiles -= DrawUIParticleLayer;
            firstFreeIndex = 0;
        }

        public override void OnWorldUnload()
        {
            for (int i = 0; i < particles.Length; i++)
            {
                particles[i] = null;
            }
            firstFreeIndex = 0;
        }

        private void DrawUIParticleLayer(ILContext il)
        {
            if (Main.dedServ) return;
            var cursor = new ILCursor(il);

            cursor.Goto(-1);

            cursor.EmitDelegate(() =>
            {
                bool anyActive = particles.Any(particle => particle != null);
                if (!anyActive) return;

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
            for (int i = 0; i < particles.Length; i++)
            {
                ParticleSF particle = particles[i];

                if (particle == null) continue;

                particle.Update();

                if (particle.time >= particle.lifetime)
                {
                    particles[i] = null;
                    if (i < firstFreeIndex)
                        firstFreeIndex = i;
                }
            }
        }

        public static void SpawnParticle(ParticleSF particle)
        {
            if (Main.dedServ || Main.gamePaused || particles == null) return;

            for (int i = firstFreeIndex; i < particles.Length; i++)
            {
                if (particles[i] != null) continue;

                particles[i] = particle;
                firstFreeIndex = i + 1;

                if (firstFreeIndex >= particles.Length)
                    firstFreeIndex = 0;
                    
                return;
            }   
        }
    }
}