using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class BloodPoison : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {

            npc.lifeRegen -= 40;
            npc.lifeRegenExpectedLossPerSecond += 20;
        
        }
    }
}
