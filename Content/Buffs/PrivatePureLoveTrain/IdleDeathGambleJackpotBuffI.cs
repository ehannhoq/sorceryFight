using System;
using CalamityMod;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PrivatePureLoveTrain
{

    public class IdleDeathGambleJackpotBuffI : ModBuff
    {
        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            sfPlayer.maxCursedEnergyFromOtherSources += 50 * sfPlayer.bossesDefeated.Count;
            sfPlayer.cursedEnergyRegenFromOtherSources += 13 * sfPlayer.bossesDefeated.Count;

            if (player.buffTime[buffIndex] <= 1)
            {
                player.GetModPlayer<SorceryFightPlayer>().idleDeathGambleBuffStrength = 0;
            }
        }
    }
}

