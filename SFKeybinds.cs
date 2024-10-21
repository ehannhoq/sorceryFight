using Terraria.ModLoader;

namespace sorceryFight 
{
    public class SFKeybinds : ModSystem
    {
        public static ModKeybind OpenTechniqueUI { get; private set; }
        public static ModKeybind UseTechnique { get; private set; }

        public override void Load()
        {
            OpenTechniqueUI = KeybindLoader.RegisterKeybind(Mod, "CursedTechniqueMenu", "C");
            UseTechnique = KeybindLoader.RegisterKeybind(Mod, "UseCursedTechnique", "Q");
        }

        public override void Unload()
        {
            OpenTechniqueUI = null;
            UseTechnique = null;
        }
    }
}