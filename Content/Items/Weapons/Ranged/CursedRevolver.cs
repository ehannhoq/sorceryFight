using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Ranged
{
    public class CursedRevolver : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.CursedReolver.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.CursedReolver.Tooltip");

        private const int CURSED_ENERGY_COST = 8;

        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 29;
            Item.maxStack = 1;
            Item.damage = 14;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ProjectileID.Bullet;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.shootSpeed = 20f;
            Item.autoReuse = true;
            Item.noMelee = true;
        }

        public override bool CanUseItem(Player player)
        {
            return player.SorceryFight().cursedEnergy > 0;
        }

        public override bool? UseItem(Player player)
        {
            player.SorceryFight().cursedEnergy -= CURSED_ENERGY_COST;
            return base.UseItem(player);
        }
    }
}