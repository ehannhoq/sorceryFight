using CalamityMod;
using CalamityMod.CalPlayer;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.PassiveTechniques.Limitless
{
    public class AmplifiedAura : PassiveTechnique
    {
        public virtual float SpeedMultiplier { get; set; } = 50f;
        public virtual float DamageMultiplier { get; set; } = 10f;
        public override string Name { get; set; } = "Amplified Aura";
        public override string Stats 
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + $"+{SpeedMultiplier}% speed boost.\n"
                        + $"+{DamageMultiplier}% damage boost.\n";
            }
        }
        public override LocalizedText Description => Language.GetText("Mods.sorceryFight.PassiveTechniques.AmplifiedAura.Description");

        public override string LockedDescription
        {
            get
            {
                return Language.GetText("Mods.sorceryFight.PassiveTechniques.AmplifiedAura.LockedDescription").Value;
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 5f;

        public override bool Unlocked
        {
            get
            {
                return Main.hardMode;
            }
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<AmplifiedAura>(), 2);
            
            if (player.HasBuff<MaximumAmplifiedAura>())
            {
                player.GetModPlayer<SorceryFightPlayer>().innateTechnique.PassiveTechniques[2].isActive = false;
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            player.moveSpeed *= (SpeedMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Melee) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Ranged) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Magic) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Summon) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(RogueDamageClass.Throwing) *= (DamageMultiplier / 100) + 1;
        } 
    }
}