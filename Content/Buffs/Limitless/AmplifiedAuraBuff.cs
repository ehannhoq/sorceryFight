using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.Content.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class AmplifiedAuraBuff : PassiveTechnique
    {
        public virtual float SpeedMultiplier { get; set; } = 50f;
        public virtual float DamageMultiplier { get; set; } = 10f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.AmplifiedAuraBuff.DisplayName");
        public override string Stats 
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + $"+{SpeedMultiplier}% speed boost.\n"
                        + $"+{DamageMultiplier}% damage boost.\n";
            }
        }
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.AmplifiedAuraBuff.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.AmplifiedAuraBuff.LockedDescription");
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return Main.hardMode;
        }

        protected Dictionary<int, int> auraIndices;
        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<AmplifiedAuraBuff>(), 2);
            
            if (player.HasBuff<MaximumAmplifiedAuraBuff>())
            {
                player.GetModPlayer<SorceryFightPlayer>().innateTechnique.PassiveTechniques[2].isActive = false;
            }

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<AmplifiedAuraProjectile>(), 0, 0, player.whoAmI);
            }

            
        }

        public override void Remove(Player player)
        {
            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (auraIndices.ContainsKey(player.whoAmI))
            { 
                Main.projectile[auraIndices[player.whoAmI]].Kill();
                auraIndices.Remove(player.whoAmI);
            }

            CostPerSecond = 10f; // Base
            
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            float newCPS = sf.maxCursedEnergy / 100 + CostPerSecond;

            if (newCPS > CostPerSecond)
                CostPerSecond = newCPS;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            CostPerSecond = 10f; // Base
            
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            float newCPS = sf.maxCursedEnergy / 100 + CostPerSecond;

            if (newCPS > CostPerSecond)
                CostPerSecond = newCPS;
                
            player.moveSpeed *= (SpeedMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Melee) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Ranged) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Magic) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(DamageClass.Summon) *= (DamageMultiplier / 100) + 1;
            player.GetDamage(RogueDamageClass.Throwing) *= (DamageMultiplier / 100) + 1;
        } 
    }
}