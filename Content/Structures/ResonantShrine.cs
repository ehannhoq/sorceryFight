using sorceryFight.StructureHelper;

namespace sorceryFight.Content.Structures
{

    public class ResonantShrine : RandomStructure
    {
        public override void SetDefaults()
        {
            Template = StructureHandler.GetStructure("ResonantShrine");
            MinDepth = 1000;
            Chance = 100;
            GenerationLimit = 1;
        }
    }
}
