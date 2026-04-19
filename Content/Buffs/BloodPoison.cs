using sorceryFight.Content.Items.Armors.Jetstream;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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
