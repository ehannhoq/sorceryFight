using sorceryFight.Content.Buffs;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class CursedEnergyMug : ModItem, ICursedEnergyPotion
    {
        public static readonly int CursedEnergy = 100;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.CursedEnergyDrinks.Mug.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.CursedEnergyDrinks.Mug.Description").WithFormatArgs(CursedEnergy);

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.consumable = true;
            Item.maxStack = 999;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.rare = ItemRarityID.Orange;
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                SorceryFightPlayer sfPlayer = player.SorceryFight();
                return sfPlayer.cursedEnergy < sfPlayer.maxCursedEnergy && !sfPlayer.HasActiveDomain && !sfPlayer.heavenlyRestriction;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                player.SorceryFight().cursedEnergy += CursedEnergy;

                if (player.FindBuffIndex(ModContent.BuffType<CursedEnergySickness>()) is int i && i != -1)
                {
                    player.buffTime[i] += SFUtils.BuffSecondsToTicks(3);
                    if (player.buffTime[i] > SFUtils.BuffSecondsToTicks(10))
                    {
                        player.buffTime[i] = SFUtils.BuffSecondsToTicks(10);
                    }
                }
                else
                    player.AddBuff(ModContent.BuffType<CursedEnergySickness>(), SFUtils.BuffSecondsToTicks(3));
            }

            return true;
        }

        public float GetCursedEnergy() => CursedEnergy;
    }
}