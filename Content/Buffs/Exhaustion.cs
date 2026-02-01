using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class Exhaustion : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Melee) *= 0.9f;
            player.moveSpeed -= 0.13f;
            player.jumpSpeedBoost -= 0.5f;
        }

        public override bool RightClick(int buffIndex)
        {
            return SorceryFight.IsDevMode();
        }
    }
}