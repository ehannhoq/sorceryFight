using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            Item.width = 70;
            Item.height = 70;
            Item.maxStack = 1;
            Item.useTime = 30;
            Item.damage = 22;
            Item.crit = 8;
            Item.knockBack = 8;
            Item.useAnimation = 30;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
