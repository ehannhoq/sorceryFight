
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.Localization;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.SFPlayer;
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Dialog;
using CalamityMod;
using Microsoft.Xna.Framework;

namespace sorceryFight.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class Gakuganji : SorceryFightNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 26;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4;
        }

        public override void SetDefaults()
        {
            SFNPC.name = "Gakuganji";
            SFNPC.attackDamage = 10;
            SFNPC.knockback = 4f;
            SFNPC.attackCooldown = 30;
            SFNPC.attackProjectile = 0;

            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;

            base.SetDefaults();
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.downedBoss1)
            {
                return true;
            }
            return false;
        }

        // public override string GetChat()
        // {
        //     WeightedRandom<string> chat = new();    
        //     chat.Add("Tempura… crisp, precise, and disciplined. Unlike horse meat… which has no place in a refined meal.");
        //     chat.Add("Hmph… these… eyes and brows don’t lie. Discipline falters when one lacks respect for the rules.");
        //     chat.Add("This instrument is not for music alone. Let its sound be the blade that strikes you down.");
        //     chat.Add("A temple of rock, applauded yet feared… I am Gakuganji Yoshinobu. Let no one mistake kindness for weakness.");
        //     chat.Add("Hendrix understood chaos… and yet controlled it with precision. Sorcery is no different.");
        //     chat.Add("A proper rhythm can change everything… even a battle. Where is the drummer to match my tempo?");
        //     chat.Add("That Gojo… an irritant wrapped in arrogance. If only he understood the weight of tradition…");
        //     string dialogKey = "Gakuganji.Interact";
        //     return chat;
        // }

        // public override void SetChatButtons(ref string button, ref string button2)
        // {
        //     button = "Cursed Items";
        //     Player player = Main.LocalPlayer;
        //     button2 = "Sukuna's Fingers";
        // }
        // public override void OnChatButtonClicked(bool firstButton, ref string shop)
        // {
        //     if (firstButton)
        //     {
        //         shop = CursedItemsShop;
        //     }
        //     if (!firstButton)
        //     {
        //         shop = SukunasFingerShop;
        //     }
        // }        

        // public override void AddShops()
        // {
        //     Condition downedHiveMindOrPerforator = CalamityConditions.DownedHiveMindOrPerforator;
        //     Condition downedCalamitasClone = CalamityConditions.DownedCalamitasClone;
        //     Condition downedLeviathan = CalamityConditions.DownedLeviathan;
        //     Condition downedRavager = CalamityConditions.DownedRavager;
        //     Condition downedAstrumDeus = CalamityConditions.DownedAstrumDeus;
        //     Condition downedDragonfolly = CalamityConditions.DownedBumblebird;
        //     Condition downedProvidence = CalamityConditions.DownedProvidence;
        //     Condition downedCeaselessVoid = CalamityConditions.DownedCeaselessVoid;
        //     Condition downedStormWeaver = CalamityConditions.DownedStormWeaver;
        //     Condition downedSignus = CalamityConditions.DownedStormWeaver;
        //     Condition downedPolterghast = CalamityConditions.DownedPolterghast;
        //     Condition downedDoG = CalamityConditions.DownedStormWeaver;

        //     var cursedItemShop = new NPCShop(Type, CursedItemsShop);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedEye>()) { shopCustomPrice = Item.buyPrice(gold: 3) }, Condition.DownedEyeOfCthulhu);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedSkull>()) { shopCustomPrice = Item.buyPrice(gold: 4, silver: 50) }, Condition.DownedSkeletron);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedFlesh>()) { shopCustomPrice = Item.buyPrice(gold: 7) }, Condition.Hardmode);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedMechanicalSoul>()) { shopCustomPrice = Item.buyPrice(gold: 10) }, Condition.DownedSkeletronPrime);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedBulb>()) { shopCustomPrice = Item.buyPrice(gold: 15) }, Condition.DownedPlantera);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedRock>()) { shopCustomPrice = Item.buyPrice(gold: 23) }, Condition.DownedGolem);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedPhantasmalEye>()) { shopCustomPrice = Item.buyPrice(gold: 29) }, Condition.DownedMoonLord);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedEffulgentFeather>()) { shopCustomPrice = Item.buyPrice(gold: 35) }, downedDragonfolly);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedProfanedShards>()) { shopCustomPrice = Item.buyPrice(gold: 42) }, downedProvidence);
        //     cursedItemShop.Add(new Item(ModContent.ItemType<CursedRuneOfKos>()) { shopCustomPrice = Item.buyPrice(gold: 50) }, downedCeaselessVoid);
        //     cursedItemShop.Register();

        //     var sukunasFingerShop = new NPCShop(Type, SukunasFingerShop);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerI>()) { shopCustomPrice = Item.buyPrice(silver: 50) }, Condition.DownedEyeOfCthulhu);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerII>()) { shopCustomPrice = Item.buyPrice(gold: 1) }, downedHiveMindOrPerforator);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerIII>()) { shopCustomPrice = Item.buyPrice(gold: 1, silver: 50) }, Condition.DownedSkeletron);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerIV>()) { shopCustomPrice = Item.buyPrice(gold: 2) }, Condition.Hardmode);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerV>()) { shopCustomPrice = Item.buyPrice(gold: 3) }, Condition.DownedSkeletronPrime);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerVI>()) { shopCustomPrice = Item.buyPrice(gold: 4) }, downedCalamitasClone);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerVII>()) { shopCustomPrice = Item.buyPrice(gold: 5) }, Condition.DownedPlantera);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerVIII>()) { shopCustomPrice = Item.buyPrice(gold: 7) }, downedLeviathan);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerIX>()) { shopCustomPrice = Item.buyPrice(gold: 9) }, Condition.DownedGolem);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerX>()) { shopCustomPrice = Item.buyPrice(gold: 12) }, downedRavager);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXI>()) { shopCustomPrice = Item.buyPrice(gold: 16) }, Condition.DownedCultist);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXII>()) { shopCustomPrice = Item.buyPrice(gold: 21) }, downedAstrumDeus);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXIII>()) { shopCustomPrice = Item.buyPrice(gold: 28) }, Condition.DownedMoonLord);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXIV>()) { shopCustomPrice = Item.buyPrice(gold: 35) }, downedDragonfolly);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXV>()) { shopCustomPrice = Item.buyPrice(gold: 42) }, downedProvidence);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXVI>()) { shopCustomPrice = Item.buyPrice(gold: 47) }, downedCeaselessVoid);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXVII>()) { shopCustomPrice = Item.buyPrice(gold: 53) }, downedStormWeaver);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXVIII>()) { shopCustomPrice = Item.buyPrice(gold: 59) }, downedSignus);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXIX>()) { shopCustomPrice = Item.buyPrice(gold: 68) }, downedPolterghast);
        //     sukunasFingerShop.Add(new Item(ModContent.ItemType<SukunasFingerXX>()) { shopCustomPrice = Item.buyPrice(gold: 75) }, downedDoG);
        //     sukunasFingerShop.Register();
        // }


    }
}