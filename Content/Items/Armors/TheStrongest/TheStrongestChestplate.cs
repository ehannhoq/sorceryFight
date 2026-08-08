using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.TheStrongest
{
    [AutoloadEquip(EquipType.Body)]
    public class TheStrongestChestplate : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.TheStrongestChestplate.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.TheStrongestChestplate.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 60;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Type && legs.type == ModContent.ItemType<TheStrongestLeggings>();
        }
    }
}