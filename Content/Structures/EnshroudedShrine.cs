using sorceryFight.StructureHelper;

namespace sorceryFight.Content.Structures
{

    public class EnshroudedShrine : RandomStructure
    {
        public override void SetDefaults()
        {
            MinDepth = 1000;
            Chance = 100;
            GenerationLimit = 1;
        }
    }
}
