using Microsoft.Xna.Framework;
using sorceryFight.StructureHelper;
using Terraria;

namespace sorceryFight.Content.Structures
{
    public class EhannShrine : RandomStructure
    {
        public override void OnLoad()
        {
            Structure.GenerationLimit = 1;
        }

        public override void PreGenerate()
        {
            Structure.GuaranteedPosition = new Point(Main.maxTilesX / 2 - Structure.Template.Width / 2, 50);
        }
    }
}
