using System;
using System.Collections.Generic;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.SupremeCalamitas;
using Humanizer;
using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Dialog;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.ObjectData;

namespace sorceryFight.Content.Tiles
{
    public class LastChallengerStatue : ModTile
    {
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

        public class LastChallengerStatueManager : ModPlayer
        {
            internal PlayerInteractions interactionProgress;

            public override void Initialize()
            {
                interactionProgress = PlayerInteractions.DidntGetArmorSet;
            }

            public override void SaveData(TagCompound tag)
            {
                tag["interactionProgress"] = (int)interactionProgress;
            }

            public override void LoadData(TagCompound tag)
            {
                interactionProgress = tag.ContainsKey("interactionProgress") ? (PlayerInteractions)tag["interactionProgress"] : PlayerInteractions.DidntGetArmorSet;
            }
        }

        internal enum PlayerInteractions : int
        {
            DidntGetArmorSet,
            GotArmorSet
        }

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
            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();
            LastChallengerStatueManager lcsManager = Main.LocalPlayer.GetModPlayer<LastChallengerStatueManager>();

            string dialogKey = "LastChallenger.Unworthy";
            bool worthy = sfPlayer.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
            bool postScal = sfPlayer.HasDefeatedBoss(ModContent.NPCType<SupremeCalamitas>());

            if (lcsManager.interactionProgress == PlayerInteractions.DidntGetArmorSet)
            {
                if (worthy)
                {
                    lcsManager.interactionProgress = PlayerInteractions.GotArmorSet;
                    dialogKey = "LastChallenger.Worthy";
                }
            }
            else
            {
                dialogKey = "LastChallenger.PreSupremeCalamitas";
                if (postScal)
                {
                    dialogKey = "LastChallenger.PostSupremeCalamitas";
                }
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
}