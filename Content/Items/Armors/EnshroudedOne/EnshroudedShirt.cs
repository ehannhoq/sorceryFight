using System;
using Terraria;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.EnshroudedOne
{
    [AutoloadEquip(EquipType.Body)]
    public class EnshroudedShirt : ModItem
    {
        public static float allDamage = 0.05f;
        public static float rctOutput = 1.5f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedShirt.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedShirt.Tooltip").WithFormatArgs((int)(allDamage * 100), (int)(rctOutput * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 60;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Generic) *= 1 + allDamage;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<EnshroudedHair>() && body.type == Type && legs.type == ModContent.ItemType<EnshroudedLeggings>();
        }
    }
}