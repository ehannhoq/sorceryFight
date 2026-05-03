using sorceryFight.Content.Particles;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight
{

    // This should be moved to .Systems or .Content.Systems along with GeneralDrawLayerSystem


    /// <summary>
    /// Calls <see cref="GeneralParticleHandler.Update"/> once per frame.
    /// This always runs regardless of whether Calamity is present, because
    /// Calamity's handler only manages its own particle types — it will
    /// never update ours.
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class ParticleUpdateSystem : ModSystem
    {
        public override void PostUpdateEverything()
        {
            if (!Main.dedServ)
                GeneralParticleHandler.Update();
        }
    }
}
