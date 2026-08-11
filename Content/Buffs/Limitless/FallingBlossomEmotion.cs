using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class FallingBlossomEmotion : PassiveTechnique
    {
        public override string InternalName => "FallingBlossomEmotion";

        public FallingBlossomEmotion()
        {
            Technique.cost = 85;
        }

        private const int DEFENSE = 20;

        protected Dictionary<int, int> auraIndices;

        public override void OnApply(Player player)
        {
            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<FallingBlossomEmotionProjectile>(), 0, 0, player.whoAmI);
            }

            player.SorceryFight().disableCurseTechniques = true;
            player.SorceryFight().fallingBlossomEmotion = true;
        }

        public override void OnRemove(Player player)
        {
            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (auraIndices.ContainsKey(player.whoAmI))
            {
                Main.projectile[auraIndices[player.whoAmI]].Kill();
                auraIndices.Remove(player.whoAmI);
            }
            SorceryFightPlayer sf = player.SorceryFight();

            sf.fallingBlossomEmotion = false;
            player.noKnockback = true;

            sf.disableCurseTechniques = false;

        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            if (player.velocity == Vector2.Zero)
            {
                player.statDefense += DEFENSE;
                player.noKnockback = true;
            }
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string baseStats =  base.GetStats(sf);
            string additionalStats = SFUtils.GetLocalization(
                "Mods.sorceryFight.PassiveTechniques.FallingBlossomEmotion.AdditionalStats")
                .WithFormatArgs(
                    DEFENSE
                ).Value;
            return baseStats + "\n" + additionalStats;
        }
    }
}
