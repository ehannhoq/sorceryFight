using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Projectiles.Melee;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class FesteringLifeBlade : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.FesteringLifeBlade.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.FesteringLifeBlade.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 29;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.damage = 56;
            Item.crit = 11;
            Item.knockBack = 2;
            Item.autoReuse = true;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.shoot = ModContent.ProjectileType<FesteringLifeBladeSlash>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.rare = ItemRarityID.Blue;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            return true;
        }
    }
}
