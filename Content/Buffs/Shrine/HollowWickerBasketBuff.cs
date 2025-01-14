using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace sorceryFight.Content.Buffs.Shrine
{
    public class HollowWickerBasketBuff : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.HollowWickerBasketBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.HollowWickerBasketBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.HollowWickerBasketBuff.LockedDescription");
        public override string Stats 
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n";
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.Golem);
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<HollowWickerBasketBuff>(), 2);   

            if (player.HasBuff<DomainAmplificationBuff>())
            {
                player.GetModPlayer<SorceryFightPlayer>().innateTechnique.PassiveTechniques[0].isActive = false;
            }
        }

        public override void Remove(Player player)
        {
            
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
        } 
    }
}