using System;
using System.Collections.Generic;
using System.Linq;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Items.Consumables;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool celestialAmulet;
        public bool pictureLocket;
        public bool cursedOfuda;
        public bool beerHat;
        public bool cursedBlindfold;

        public override void ResetEffects()
        {
            celestialAmulet = false;
            pictureLocket = false;
            cursedOfuda = false;
            beerHat = false;
            cursedBlindfold = false;
            onDeath = false;
        }

        private bool BeerHatRecoverCE(float minRecover = -1f)
        {
            if (HasActiveDomain) return false;

            int strongestCEIndex = -1;
            float strongestCE = 0f;

            bool canRecoverCEAgain = false;

            for (int i = 0; i < Player.inventory.Length; i++)
            {
                Item item = Player.inventory[i];
                if (ModContent.GetModItem(item.type) is ModItem modItem && modItem != null && modItem is ICursedEnergyPotion)
                {
                    ICursedEnergyPotion potion = modItem as ICursedEnergyPotion;

                    if (item.stack > 1)
                        canRecoverCEAgain = true;

                    if (potion.GetCursedEnergy() > strongestCE)
                    {
                        strongestCEIndex = i;
                        strongestCE = potion.GetCursedEnergy();
                    }
                }
            }


            if (strongestCEIndex != -1)
            {
                cursedEnergy += strongestCE;
                Player.inventory[strongestCEIndex].stack--;

                if (Player.FindBuffIndex(ModContent.BuffType<CursedEnergySickness>()) is int i && i != -1)
                {
                    Player.buffTime[i] += SFUtils.BuffSecondsToTicks(3);
                    if (Player.buffTime[i] > SFUtils.BuffSecondsToTicks(10))
                    {
                        Player.buffTime[i] = SFUtils.BuffSecondsToTicks(10);
                    }
                }
                else
                {
                    Player.AddBuff(ModContent.BuffType<CursedEnergySickness>(), SFUtils.BuffSecondsToTicks(4));
                }
            }

            if (minRecover - strongestCE > 0 && canRecoverCEAgain)
            {
                BeerHatRecoverCE(minRecover - strongestCE);
            }

            return strongestCEIndex != -1;
        }
    }
}