using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class PotentialSword : ModItem
    { 
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.PotentialSword.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.PotentialSword.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 43;
            Item.height = 88;
            Item.maxStack = 1;
            Item.useTime = 12;
            Item.damage = 18;
            Item.crit = 15;
            Item.knockBack = 4;
            Item.useAnimation = 12;
            Item.autoReuse = true;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
