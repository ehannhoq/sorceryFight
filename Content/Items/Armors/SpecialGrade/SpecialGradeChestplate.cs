using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.SpecialGrade
{
    [AutoloadEquip(EquipType.Body)]
    public class SpecialGradeChestplate : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeChestplate.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeChestplate.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 60;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Type && legs.type == ModContent.ItemType<SpecialGradeLeggings>();
        }
    }
}