using Terraria.ModLoader;

namespace sorceryFight 
{
    public class SFKeybinds : ModSystem
    {
        public static ModKeybind OpenTechniqueUI { get; private set; }
        public static ModKeybind UseTechnique { get; private set; }
        public static ModKeybind DomainExpansion { get; private set;}
        public static ModKeybind UseRCT { get; private set; }
        public override void Load()
        {
            OpenTechniqueUI = KeybindLoader.RegisterKeybind(Mod, "CursedTechniqueMenu", "C");
            UseTechnique = KeybindLoader.RegisterKeybind(Mod, "UseCursedTechnique", "Q");
            DomainExpansion = KeybindLoader.RegisterKeybind(Mod, "DomainExpansion", "F");
            UseRCT = KeybindLoader.RegisterKeybind(Mod, "UseRCT", "X");
        }

        public override void Unload()
        {
            OpenTechniqueUI = null;
            UseTechnique = null;
            DomainExpansion = null;
            UseRCT = null;
        }
    }
}