using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.DevItems
{
    public class ClearQuests : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DevItems.ClearQuests.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.DevItems.ClearQuests.Tooltip");
        
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.completedQuests = new();
            sfPlayer.currentQuests = new();

            return true;
        }
    }
}