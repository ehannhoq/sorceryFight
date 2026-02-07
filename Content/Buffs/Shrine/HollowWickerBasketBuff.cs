using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using sorceryFight.Content.Buffs.Vessel;

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
                return "Base CE Consumption: 50 CE/s\n"
                        + "100% of incoming damage is\n"
                        + "neutralized into Cursed Energy.\n"
                        + "Grants immunity to enemy domains.\n"            
                        + "You cannot use Cursed Techniques while,\n"
                        + "this is active, as well as moving 10% slower,\n"
                        + "unless you have a unique body structure.\n"
                       + "Hollow Wicker Basket takes 3x more CE during boss fights.\n";
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 50f;
        public Dictionary<int, int> auraIndices;
        public bool waiting = false;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.Golem) || sf.Player.HasBuff(ModContent.BuffType<KingOfCursesBuff>());
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<HollowWickerBasketBuff>(), 2);

            SorceryFightPlayer sfPlayer = player.SorceryFight();

            if (player.HasBuff<DomainAmplificationBuff>())
            {
                sfPlayer.innateTechnique.PassiveTechniques[0].isActive = false;
            }

            sfPlayer.hollowWickerBasket = true;

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<HollowWickerBasketProjectile>(), 0, 0, player.whoAmI);

            }

            if (!sfPlayer.uniqueBodyStructure)
                player.moveSpeed -= 0.10f;

            sfPlayer.disableCurseTechniques = true;
        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.hollowWickerBasket = false;


            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (auraIndices.ContainsKey(player.whoAmI))
            {
                Main.projectile[auraIndices[player.whoAmI]].Kill();
                auraIndices.Remove(player.whoAmI);
            }
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            float minimumDistance = 25f;
            float accumulativeDamage = 0f;

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile) continue;

                float distance = Vector2.DistanceSquared(proj.Center, player.Center);
                if (distance <= minimumDistance * minimumDistance)
                {
                    accumulativeDamage += proj.damage;
                }
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.type == NPCID.TargetDummy) continue;

                float distance = Vector2.DistanceSquared(npc.Center, player.Center);
                if (distance <= minimumDistance * minimumDistance)
                {
                    accumulativeDamage += npc.damage;
                }
            }

            if (accumulativeDamage > 0f && !waiting)
            {

                TaskScheduler.Instance.AddContinuousTask(() =>
                {
                    sfPlayer.disableRegenFromBuffs = true;
                },
                300);

                TaskScheduler.Instance.AddDelayedTask(() =>
                {
                    waiting = false;
                },
                301);

                waiting = true;
            }

            int multiplier = 1;
            // if (CalamityMod.CalPlayer.CalamityPlayer.areThereAnyDamnBosses)
            // {
            //     multiplier = 3;
            // }

            CostPerSecond = 50f;
            CostPerSecond += accumulativeDamage * 3f * multiplier;

            base.Update(player, ref buffIndex);            
        }
    }
}