using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class MaximumAmplifiedAuraBuff : AmplifiedAuraBuff
    {
        public override float SpeedMultiplier { get; set; } = 100f;
        public override float DamageMultiplier { get; set; } = 50f;
        
        public override LocalizedText DisplayName { get; } = SFUtils.GetLocalization("Mods.sorceryFight.Buffs.MaximumAmplifiedAuraBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.MaximumAmplifiedAuraBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.MaximumAmplifiedAuraBuff.LockedDescription");
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 10f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return NPC.downedGolemBoss;
        }
        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<MaximumAmplifiedAuraBuff>(), 2);

            if (player.HasBuff<AmplifiedAuraBuff>())
            {
                player.GetModPlayer<SorceryFightPlayer>().innateTechnique.PassiveTechniques[1].isActive = false;
            }

            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<MaximumAmplifiedAuraProjectile>(), 0, 0, player.whoAmI);
            }
        }
    }
}