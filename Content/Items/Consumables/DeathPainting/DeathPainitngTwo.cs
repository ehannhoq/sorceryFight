using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables.DeathPainting
{
    public class DeathPaintingTwo : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.DeathPaintingTwo.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.DeathPaintingTwo.Description");
        public override void SetStaticDefaults()
        {
            //Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
        }
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Master;
        }

        public override bool CanUseItem(Player player)
        {
            string techName = player.SorceryFight().innateTechnique.Name;
            return techName == "Vessel" || techName == "BloodManipulation";
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
               //SoundEngine.PlaySound(SoundID.Item4);
                SorceryFightPlayer sf = player.SorceryFight();
                sf.deathPaintingTwo = true;
            }
            return true;
        }
    }
}