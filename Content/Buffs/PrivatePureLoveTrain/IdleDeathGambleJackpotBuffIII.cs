using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PrivatePureLoveTrain
{
    public class IdleDeathGambleJackpotBuffIII : ModBuff
    {
        private Dictionary<int, int> auraIndices = new Dictionary<int, int>();

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            if (player.buffType.ToList<int>().Contains(ModContent.BuffType<BurntTechnique>()))
            {
                int index = -1;
                for (int i = 0; i < player.buffType.Length; i++)
                {
                    if (player.buffType[i] == ModContent.BuffType<BurntTechnique>())
                    {
                        index = i;
                    }
                }
                if (index != -1)
                    player.DelBuff(index);
            }

            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            sfPlayer.maxCursedEnergyFromOtherSources += 100 * sfPlayer.numberBossesDefeated;
            sfPlayer.cursedEnergyRegenFromOtherSources += 25 * sfPlayer.numberBossesDefeated;

            player.Heal(2);
            ApplyAura(player);

            if (player.buffTime[buffIndex] <= 1)
            {
                auraIndices.Remove(player.whoAmI);
            }
        }

        private void ApplyAura(Player player)
        {
            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<IdleDeathGambleJackpotProjectileIII>(), 0, 0, player.whoAmI);
            }
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
