using Terraria.ModLoader;

namespace sorceryFight.Content
{
    public class CursedTechniqueDamageClass : DamageClass
    {
        internal static CursedTechniqueDamageClass Instance;

        public override void Load() => Instance = this;
        public override void Unload() => Instance = null;
    }
}


