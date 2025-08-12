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
        public override void SetStaticDefaults()
        {
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

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
            sfPlayer.cursedEnergyRegenFromOtherSources += 100;
            sfPlayer.unlockedRCT = true;

            if (player.buffTime[buffIndex] < 1)
            {
                foreach (PassiveTechnique pt in sfPlayer.innateTechnique.PassiveTechniques)
                {
                    pt.isActive = false;
                }

                sfPlayer.selectedTechnique = null;

                sfPlayer.innateTechnique = new VesselTechnique();
                if (rctTracker.TryGetValue(player.whoAmI, out bool hasRCT))
                {
                    sfPlayer.unlockedRCT = hasRCT;
                    rctTracker.Remove(player.whoAmI);
                }

                sfPlayer.hollowWickerBasket = false;
                sfPlayer.domainAmp = false;

                SorceryFightUI.UpdateTechniqueUI.Invoke();
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }

        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.buffTime[buffIndex] = 2;
            return true;
        }
    }
}
