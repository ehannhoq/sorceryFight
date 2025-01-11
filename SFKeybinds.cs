using Terraria.ModLoader;

namespace sorceryFight 
{
    public class SFKeybinds : ModSystem
    {
        public static ModKeybind OpenTechniqueUI { get; private set; }
        public static ModKeybind UseTechnique { get; private set; }
        public static ModKeybind CycleSelectedTechniqueUp { get; private set; }
        public static ModKeybind CycleSelectedTechniqueDown { get; private set; }

        public static ModKeybind DomainExpansion { get; private set;}
        public static ModKeybind UseRCT { get; private set; }
        public override void Load()
        {
            OpenTechniqueUI = KeybindLoader.RegisterKeybind(Mod, "CursedTechniqueMenu", "T");
            UseTechnique = KeybindLoader.RegisterKeybind(Mod, "UseCursedTechnique", "F");
            CycleSelectedTechniqueUp = KeybindLoader.RegisterKeybind(Mod, "CycleSelectedTechniqueForward", "Q");
            CycleSelectedTechniqueDown = KeybindLoader.RegisterKeybind(Mod, "UseCursedTechniqueBackward", "Z");
            DomainExpansion = KeybindLoader.RegisterKeybind(Mod, "DomainExpansion", "G");
            UseRCT = KeybindLoader.RegisterKeybind(Mod, "UseRCT", "X");
        }

        public override void Unload()
        {
            OpenTechniqueUI = null;
            UseTechnique = null;
            CycleSelectedTechniqueUp = null;
            CycleSelectedTechniqueDown = null;
            DomainExpansion = null;
            UseRCT = null;
        }
    }
}