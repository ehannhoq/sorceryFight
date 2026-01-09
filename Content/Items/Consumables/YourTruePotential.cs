using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
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
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
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

            sfPlayer.AddDefeatedBoss(ModContent.NPCType<DesertScourgeHead>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Crabulon>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<HiveMind>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<PerforatorHive>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<EbonianPaladin>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<CrimulanPaladin>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Cryogen>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<AquaticScourgeHead>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<BrimstoneElemental>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<CalamitasClone>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Leviathan>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Anahita>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<AstrumAureus>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<PlaguebringerGoliath>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<RavagerHead>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<AstrumDeusHead>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<ProfanedGuardianCommander>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<ProfanedGuardianHealer>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<ProfanedGuardianDefender>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Providence>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<StormWeaverHead>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<CeaselessVoid>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Signus>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Polterghast>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<OldDuke>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Yharon>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<ThanatosHead>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Artemis>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<AresBody>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<Apollo>());
            sfPlayer.AddDefeatedBoss(ModContent.NPCType<SupremeCalamitas>());

            return true;
        }
    }
}