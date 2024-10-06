using Terraria.ModLoader;

namespace sorceryFight 
{
    public class SFKeybinds : ModSystem
    {
        public static ModKeybind OpenTechniqueUI { get; private set; }

        public override void Load()
        {
            OpenTechniqueUI = KeybindLoader.RegisterKeybind(Mod, "OpenTechniqueUI", "C");
        }

        public override void Unload()
        {
            OpenTechniqueUI = null;
        }
    }
}