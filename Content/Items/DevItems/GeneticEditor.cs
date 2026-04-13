using System.Linq;
using sorceryFight.Content.UI.GeneticEditor;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.DevItems
{
    public class GeneticEditor : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DevItems.GeneticEditor.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.DevItems.GeneticEditor.Tooltip");
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            if (sfPlayer.sfUI.Children.Any(x => x is GeneticEditorUI)) return true;
            sfPlayer.sfUI.GeneticEditorUI();
            return true;
        }
    }
}