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
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            sfPlayer.maxCursedEnergyFromOtherSources += 75 * sfPlayer.bossesDefeated.Count;
            sfPlayer.cursedEnergyRegenFromOtherSources += 25 * sfPlayer.bossesDefeated.Count;

            player.Heal(1);

            if (player.buffTime[buffIndex] <= 1)
            {
                player.GetModPlayer<SorceryFightPlayer>().idleDeathGambleBuffStrength = 0;
            }
        }
    }
}
