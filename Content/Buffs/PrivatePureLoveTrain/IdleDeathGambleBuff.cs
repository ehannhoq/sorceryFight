using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PrivatePureLoveTrain
{
    public class IdleDeathGambleBuff : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            int multiplier = player.SorceryFight().idleDeathGambleBuffStrength;

            player.moveSpeed *= 1 + multiplier / 10f; 
            player.GetDamage(DamageClass.Melee) *= 1 + multiplier / 20f;
            player.GetDamage(DamageClass.Ranged) *= 1 + multiplier / 20f;
            player.GetDamage(DamageClass.Magic) *= 1 + multiplier / 20f;
            player.GetDamage(DamageClass.Summon) *= 1 + multiplier / 20f;
            player.statDefense *= 1 + multiplier / 10f;
            player.lifeRegen += multiplier + 1;

            if (player.buffTime[buffIndex] <= 1)
            {
                player.SorceryFight().idleDeathGambleBuffStrength = 0;
            }
        }
    }
}
