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
    public class MeiMeiAxe : ModItem
    { 
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.MeiMeiAxe.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.MeiMeiAxe.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 113;
            Item.maxStack = 1;
            Item.damage = 160;
            Item.crit = 30;
            Item.knockBack = 10;
            Item.useTime = 70;
            Item.useAnimation = 70;
            Item.shoot = ModContent.ProjectileType<MeiMeiAxeHoldout>();
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.rare = ItemRarityID.Blue;
            Item.noMelee = true;
            Item.channel = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.shootSpeed = 24f;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            return true;
        }
    }
}
