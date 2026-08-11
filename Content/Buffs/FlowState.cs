using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class FlowState : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
        }
    }
}
