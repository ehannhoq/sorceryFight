using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PrivatePureLoveTrain
{
    public class IdleDeathGambleJackpotBuffII : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            sfPlayer.maxCursedEnergyFromOtherSources += 75 * sfPlayer.numberBossesDefeated;
            sfPlayer.cursedEnergyRegenFromOtherSources += 25 * sfPlayer.numberBossesDefeated;

            player.Heal(1);

            if (player.buffTime[buffIndex] <= 1)
            {
                player.SorceryFight().idleDeathGambleBuffStrength = 0;
            }
        }
    }
}
