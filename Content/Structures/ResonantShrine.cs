using sorceryFight.StructureHelper;

namespace sorceryFight.Content.Structures
{
    public class ResonantShrine : RandomStructure
    {
        public override void OnLoad()
        {
            Structure.MinDepth = 1000;
            Structure.Chance = 1f;
            Structure.GenerationLimit = 1;
        }
    }
}
