using System;
using System.Collections.Generic;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.SupremeCalamitas;
using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Dialog;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace sorceryFight.Content.Tiles
{
    public class LastChallengerStatue : ModTile
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

            string dialogKey = "LastChallenger.Unworthy";
            bool worthy = sfPlayer.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>()) && sfPlayer.HasDefeatedBoss(ModContent.NPCType<SupremeCalamitas>());

            if (worthy)
            {
                if (!grantedSets.ContainsKey(sfPlayer.Player.name))
                {
                    dialogKey = "LastChallenger.Worthy";
                    grantedSets.Add(sfPlayer.Player.name, true);
                }
                else if (!sfPlayer.HasDefeatedBoss(ModContent.NPCType<SupremeCalamitas>()))
                    dialogKey = "LastChallenger.PreSupremeCalamitas";
                else
                    dialogKey = "LastChallenger.PostSupremeCalamitas";
            }


            ModContent.GetInstance<SorceryFightUISystem>().ActivateDialogUI(Dialog.Create(dialogKey), this);
            return true;
        }

        public void GrantQuantumCoulombSet()
        {
            var player = Main.LocalPlayer;
            player.QuickSpawnItem(player.GetSource_Misc("QuantumCoulombBottle"), ModContent.ItemType<QuantumCoulombBottle>());
            player.QuickSpawnItem(player.GetSource_Misc("QuantumCoulombBodyArmor"), ModContent.ItemType<QuantumCoulombBodyArmor>());
            player.QuickSpawnItem(player.GetSource_Misc("QuantumCoulombChausses"), ModContent.ItemType<QuantumCoulombChausses>());
            player.QuickSpawnItem(player.GetSource_Misc("SuspiciouslyWellPerservedEye"), ModContent.ItemType<SuspiciouslyWellPerservedEye>());
        }
    }

    public class LastChallengerStatueItem : ModItem
    {
        public override string Texture => "sorceryFight/Content/Tiles/LastChallengerStatue";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 48;
            Item.createTile = ModContent.TileType<LastChallengerStatue>();
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