using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.EnshroudedOne
{
    [AutoloadEquip(EquipType.Legs)]
    public class EnshroudedLeggings : ModItem
    {
        public static float limitlessDamage = 0.05f;
        public static float allDamage = 0.05f;
        public static float rctOutput = 1.0f;
        public static float movementSpeed = 0.15f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedLeggings.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedLeggings.Tooltip").WithFormatArgs((int)(limitlessDamage * 100), (int)(allDamage * 100), (int)(rctOutput * 100), (int)(movementSpeed * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 55;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            if (sfPlayer.innateTechnique.Name == "Limitless")
                player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + limitlessDamage;

            player.GetDamage(DamageClass.Generic) *= 1 + allDamage;

            player.moveSpeed *= 1 + movementSpeed;
        }
    }
}