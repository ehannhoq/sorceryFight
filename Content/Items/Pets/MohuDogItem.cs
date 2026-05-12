using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.Pets;
using sorceryFight.Content.Projectiles.Pets;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Pets
{
    public class MohuDogItem : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Pets.MohuDogItem.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Pets.MohuDogItem.Description");

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.UnluckyYarn);

            Item.shoot = ModContent.ProjectileType<MohuDogProjectile>();
            Item.buffType = ModContent.BuffType<MohuDogBuff>();
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.DirtBlock, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            if (player.whoAmI == Main.myPlayer && player.itemTime == 0)
            {
                player.AddBuff(Item.buffType, 3600);
            }
        }
    }
}

