using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class CursedEnergyCoin : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.CursedEnergyCoin.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.CursedEnergyCoin.Tooltip");
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<SorceryFightWeapon>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SorceryFightPlayer sf = player.SorceryFight();                
                sf.RollForCursedEnergyTraits(true);
            }
            return true;
        }
    }
}