using sorceryFight.SFPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class YourTruePotential : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.YourTruePotential.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.YourTruePotential.Tooltip");
        
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(8, 4));
        }
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.unlockedRCT = true;
            
            sfPlayer.AddDefeatedBoss(NPCID.KingSlime);
            sfPlayer.AddDefeatedBoss(NPCID.EyeofCthulhu);
            sfPlayer.AddDefeatedBoss(NPCID.EaterofWorldsHead);
            sfPlayer.AddDefeatedBoss(NPCID.BrainofCthulhu);
            sfPlayer.AddDefeatedBoss(NPCID.QueenBee);
            sfPlayer.AddDefeatedBoss(NPCID.SkeletronHead);
            sfPlayer.AddDefeatedBoss(NPCID.Deerclops);
            sfPlayer.AddDefeatedBoss(NPCID.WallofFlesh);
            sfPlayer.AddDefeatedBoss(NPCID.QueenSlimeBoss);
            sfPlayer.AddDefeatedBoss(NPCID.Retinazer);
            sfPlayer.AddDefeatedBoss(NPCID.Spazmatism);
            sfPlayer.AddDefeatedBoss(NPCID.TheDestroyer);
            sfPlayer.AddDefeatedBoss(NPCID.SkeletronPrime);
            sfPlayer.AddDefeatedBoss(NPCID.Plantera);
            sfPlayer.AddDefeatedBoss(NPCID.Golem);
            sfPlayer.AddDefeatedBoss(NPCID.DukeFishron);
            sfPlayer.AddDefeatedBoss(NPCID.HallowBoss);
            sfPlayer.AddDefeatedBoss(NPCID.CultistBoss);
            sfPlayer.AddDefeatedBoss(NPCID.MoonLordCore);
            return true;
        }
    }
}