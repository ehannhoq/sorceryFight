using Terraria.ModLoader;

namespace sorceryFight 
{
    public class SFKeybinds : ModSystem
    {
        public static ModKeybind OpenTechniqueUI { get; private set; }

        public override void Load()
        {
            OpenTechniqueUI = KeybindLoader.RegisterKeybind(Mod, "CursedTechniqueMenu", "C");
        }

        public override void Unload()
        {
            OpenTechniqueUI = null;
        }
    }
}