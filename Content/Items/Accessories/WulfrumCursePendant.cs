using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Items.Materials;

namespace sorceryFight.Content.Items.Accessories
{
    // ================= ITEM =================
    public class WulfrumCursePendant : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.WulfrumCursePendant.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.WulfrumCursePendant.Tooltip");

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            var sfPlayer = player.GetModPlayer<WulfrumCursePendantPlayer>();
            sfPlayer.equipped = true;

            player.GetModPlayer<SorceryFightPlayer>().cursedEnergyRegenFromOtherSources += 2;
            player.GetModPlayer<SorceryFightPlayer>().maxCursedEnergyFromOtherSources += 50;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 6);
            recipe.AddIngredient(ModContent.ItemType<WulfrumMetalScrap>(), 4);
            recipe.AddIngredient(ItemID.Diamond);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }

    // ================= DROP LOGIC =================
    public class WulfrumCursePendantNPC : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (!npc.GetGlobalNPC<SorceryNPC>().isWulfrum)
                return;

            Player player = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];
            if (!player.GetModPlayer<WulfrumCursePendantPlayer>().equipped)
                return;

            if (Main.rand.NextFloat() < 0.10f)
            {
                Item.NewItem(
                    npc.GetSource_Loot(),
                    npc.getRect(),
                    ModContent.ItemType<CursedFragment>()
                );
            }
        }
    }

    // ================= PLAYER FLAG =================
    public class WulfrumCursePendantPlayer : ModPlayer
    {
        public bool equipped;

        public override void ResetEffects()
        {
            equipped = false;
        }
    }
}
