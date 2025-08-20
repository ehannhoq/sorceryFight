using System;
using System.Collections.Generic;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Armors.EnshroudedOne;
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Dialog;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace sorceryFight.Content.Tiles
{
    public class EnshroudedOneStatue : ModTile
    {
        public Dictionary<string, bool> grantedSets = new Dictionary<string, bool>();
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            MinPick = 400;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.LavaDeath = false;

            TileObjectData.addTile(Type);
        }

        public override bool RightClick(int i, int j)
        {
            SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();

            string dialogKey = "EnshroudedOne.Unworthy";
            bool worthy = sfPlayer.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>()) && sfPlayer.HasDefeatedBoss(ModContent.NPCType<SupremeCalamitas>());

            if (worthy)
            {
                if (!grantedSets.ContainsKey(sfPlayer.Player.name))
                {
                    dialogKey = "EnshroudedOne.Worthy";
                    grantedSets.Add(sfPlayer.Player.name, true);
                }
                else
                    dialogKey = "EnshroudedOne.PreBossRush";
            }


            ModContent.GetInstance<SorceryFightUISystem>().ActivateDialogUI(Dialog.Create(dialogKey), this);
            return true;
        }

        public void GrantEnshroudedSet()
        {
            var player = Main.LocalPlayer;
            player.QuickSpawnItem(player.GetSource_Misc("EnshroudedHair"), ModContent.ItemType<EnshroudedHair>());
            player.QuickSpawnItem(player.GetSource_Misc("EnshroudedShirt"), ModContent.ItemType<EnshroudedShirt>());
            player.QuickSpawnItem(player.GetSource_Misc("EnshroudedHaori"), ModContent.ItemType<EnshroudedHaori>());
            player.QuickSpawnItem(player.GetSource_Misc("EnshroudedLeggings"), ModContent.ItemType<EnshroudedLeggings>());
            player.QuickSpawnItem(player.GetSource_Misc("EnshroudedPants"), ModContent.ItemType<EnshroudedPants>());
        }
    }

    public class EnshroudedOneStatueItem : ModItem
    {
        public override string Texture => "sorceryFight/Content/Tiles/EnshroudedOneStatue";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 48;
            Item.createTile = ModContent.TileType<EnshroudedOneStatue>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.maxStack = 99;
            Item.consumable = true;
        }
    }
}