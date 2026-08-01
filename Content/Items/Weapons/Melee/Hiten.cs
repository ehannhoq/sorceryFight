using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class Hiten : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 29;
            Item.maxStack = 1;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 10;
            Item.damage = 56;
            Item.crit = 11;
            Item.knockBack = 8;
            Item.autoReuse = true;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            // Item.shoot = ModContent.ProjectileType<CursedSpearProjectile>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item7;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            return true;
        }
    }
}
