using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace sorceryFight.Content.Buffs.HeavenlyRestriction
{
    public class InorganicPerception : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.InorganicPerception.DisplayName");
        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + "You must be standing still in order to recieve the buffs.\n"
                        + "Grants immunity to enemy domains.\n"
                        + "Grants +20 defense.\n"
                        + "Grants immunity to knockback.\n"
                        + "You cannot use Cursed Techniques while this is active,\n"
                        + "unless you have a unique body structure.\n";
            }
        }
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.InorganicPerception.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.InorganicPerception.LockedDescription");
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 85;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.Golem);
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<InorganicPerception>(), 2);

            if (player.HasBuff<MindlessCarnage>())
            {
                player.SorceryFight().innateTechnique.PassiveTechniques[0].isActive = false;
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