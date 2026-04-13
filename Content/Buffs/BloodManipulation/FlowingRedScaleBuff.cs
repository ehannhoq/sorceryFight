using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.HiveMind;

//remove Later

namespace sorceryFight.Content.Buffs.BloodManipulation
{
    public class FlowingRedScaleBuff : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.FlowingRedScaleBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.FlowingRedScaleBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.FlowingRedScaleBuff.LockedDescription");

        public override bool isActive { get; set; } = false;
        public override float BloodRegenPerSecond { get; set; } = 10;

        public override float CostPerSecond { get; set; } = 10f;

        float BossMultiplier = 1.5f;
        public virtual int DefenseAddition { get; set; } = 12;
        public virtual float DamageNegation { get; set; } = 0.10f;

        public override bool isAura => true;

        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + $"+{DefenseAddition}% defense boost.\n"
                        + $"+{DamageNegation}% damage negation.\n"
                        + $"Generates 10 BE/s \n"
                        + "If you don't have RCT it will consume health instead\n"
                        + "You cannot use Cursed Techniques while this is active,\n"
                        + "unless you have a unique body structure.\n"
                        + $"Flowing Red Scale takes +{BossMultiplier}x more CE during boss fights.\n";
            }
        }


        public Dictionary<int, int> auraIndices;

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<FlowingRedScaleBuff>(), 2);
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            //if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            //{
            //    Vector2 playerPos = player.MountedCenter;
            //    var entitySource = player.GetSource_FromThis();

            //    auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<DomainAmplificationProjectile>(), 0, 0, player.whoAmI);
            //}

            sfPlayer.disableCurseTechniques = true;
        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.domainAmp = false;

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (auraIndices.ContainsKey(player.whoAmI))
            {
                Main.projectile[auraIndices[player.whoAmI]].Kill();
                auraIndices.Remove(player.whoAmI);
            }
            
            sfPlayer.disableCurseTechniques = false;
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            if (sf.innateTechnique.Name == "Vessel")
            {
                return sf.sukunasFingerConsumed >= 10;
            }
            else
            {
                return sf.HasDefeatedBoss(ModContent.NPCType<PerforatorHive>()) || sf.HasDefeatedBoss(ModContent.NPCType<HiveMind>());

            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            player.endurance += DamageNegation;
            player.statDefense += DefenseAddition;
            
            float multiplier = 1;
            if (CalamityMod.CalPlayer.CalamityPlayer.areThereAnyDamnBosses)
            {
                multiplier = BossMultiplier;
            }

            /**
            if (sfPlayer.unlockedRCT)
            {
                CostPerSecond = 10f;
                CostPerSecond += CostPerSecond * multiplier;
            }
            else
            {
                CostPerSecond = 0;
                player.lifeRegen -= 10;
            }



            **/
            if (sfPlayer.unlockedRCT)
            {
                CostPerSecond = 10f;
                CostPerSecond += CostPerSecond * multiplier;
            }
            else
            {
                CostPerSecond = 0;
                player.lifeRegen -= 10;
            }

            CostPerSecond += CostPerSecond * multiplier;

            base.Update(player, ref buffIndex);
        }
    }
}
