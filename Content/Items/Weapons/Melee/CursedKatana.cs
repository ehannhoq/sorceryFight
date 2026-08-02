using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class CursedKatana : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.CursedKatana.DisplayName");

        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.CursedKatana.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.scale = 0.65f;
            Item.maxStack = 1;
            Item.useTime = 13;
            Item.useAnimation = 13;
            Item.damage = 17;
            Item.knockBack = 5;
            Item.autoReuse = true;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
        }
    }
}
