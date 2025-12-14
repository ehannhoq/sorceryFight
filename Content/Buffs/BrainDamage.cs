using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class BrainDamage : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return SorceryFight.IsDevMode();
        }
    }
}