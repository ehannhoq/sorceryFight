using System;
using System.Collections.Generic;
using CalamityMod;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Vessel
{
    public class KingOfCursesBuff : ModBuff
    {
        private static Dictionary<int, bool> rctTracker = new Dictionary<int, bool>();

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            if (player.buffTime[buffIndex] < 1)
            {
                sfPlayer.innateTechnique = new VesselTechnique();
                sfPlayer.maxCursedEnergy = sfPlayer.calculateBaseCERegenRate();
                SorceryFightUI.UpdateTechniqueUI.Invoke();

                sfPlayer.unlockedRCT = rctTracker.ContainsKey(player.whoAmI) ? rctTracker[player.whoAmI] : false;
                rctTracker.Remove(player.whoAmI);
            }

            if (sfPlayer.innateTechnique.Name.Equals("Vessel"))
            {
                sfPlayer.innateTechnique = new ShrineTechnique();
                rctTracker[player.whoAmI] = sfPlayer.unlockedRCT;
                SorceryFightUI.UpdateTechniqueUI.Invoke();
            }

            if (player.statLife > (int)(player.statLifeMax2 * 0.2f))
            {
                player.statLife = (int)(player.statLifeMax2 * 0.2f);
            }

            sfPlayer.maxCursedEnergyFromOtherSources += 1500;
            sfPlayer.cursedEnergyRegenFromOtherSources += 50;
            sfPlayer.unlockedRCT = true;
        }
    }
}
