using sorceryFight.Content.Rarities;
using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public abstract class SukunasFingerBase : ModItem
    {
        public abstract int Id { get; }
        public override string Texture => "sorceryFight/Content/Items/Consumables/SukunasFinger/SukunasFinger";
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.SukunasFinger.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.SukunasFinger.Tooltip");

        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<SorceryFightConsumable>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();

                if (!sf.innateTechnique.Name.Equals("Shrine") && !sf.innateTechnique.Name.Equals("Vessel")) return false;

                if (!sf.sukunasFingers[Id - 1])
                {
                    sf.sukunasFingers[Id - 1] = true;
                    SoundEngine.PlaySound(SoundID.Item2, player.Center);

                    if (sf.innateTechnique.Name.Equals("Vessel"))
                        SorceryFightUI.UpdateTechniqueUI.Invoke();
                }
                return true;
            }
            return false;
        }
    }
}