using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class CursedSpear : ModItem // NEED TO IMPLEMENT PROJECTILE VERSION OF THIS TO ACTUALLY WORK
    {
        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 29;
            Item.maxStack = 1;
            Item.useTime = 10;
            Item.damage = 56;
            Item.crit = 11;
            Item.knockBack = 8;
            Item.autoReuse = true;
            Item.useAnimation = 10;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item7;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = true;
        }
    }
}
