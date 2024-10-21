using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.PassiveTechniques
{
    public class BurntTechnique : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }


    }
}