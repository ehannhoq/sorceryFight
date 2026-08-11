using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.StarRage
{
    public class SummonGaruda : PassiveTechnique
    {
        public override string InternalName => "SummonGaruda";

        public SummonGaruda()
        {
            Technique.cost = 10;
        }

        //code made using Calamity's Tenryu as an example, HEAVILY modified to fit PassiveTechnique structure

        private const int DAMAGE = 20;
        private const int BOSS_MULTIPLIER = 3;

        public override void OnApply(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            Vector2 spawnPos = new Vector2(player.position.X - 200, player.position.Y - 200);
            Vector2 spawnPos2 = new Vector2(player.position.X + 200, player.position.Y - 200);
            SummonGarudaEntity(ModContent.ProjectileType<GarudaHead>(), ModContent.ProjectileType<GarudaBody>(), ModContent.ProjectileType<GarudaTail>(), spawnPos, player, player.GetSource_FromThis(), DAMAGE, 0);
            sfPlayer.summonGaruda = true;
        }

        public override void OnRemove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.summonGaruda = false;
        }

        public static void SummonGarudaEntity(int headType, int bodyType, int tailType, Vector2 spawnPos, Player player, IEntitySource source, int damage, float knockback)
        {
            var head = Projectile.NewProjectileDirect(source, spawnPos, player.DirectionTo(Main.MouseWorld) * 3, headType, damage, knockback, player.whoAmI);
            var tail = Projectile.NewProjectileDirect(source, spawnPos, new Vector2(0f, -0.01f), tailType, damage, knockback, player.whoAmI);
            for (var i = 0; i < 20; i++)
            {
                var body = Projectile.NewProjectileDirect(source, spawnPos, new Vector2(0f, -0.01f), bodyType, damage, knockback, player.whoAmI);
            }
        }

        public override bool CanUse(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<GarudaCooldown>());
        }


        public override void Update(Player player, ref int buffIndex)
        {

            int multiplier = 1;
            if (AreThereAnyDamnBosses.BossActive)
            {
                multiplier = BOSS_MULTIPLIER;
            }

            Technique.cost = 10f;
            Technique.cost *= multiplier;

            base.Update(player, ref buffIndex);
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string baseStats = base.GetStats(sf);
            string additionalStats = SFUtils.GetLocalization(
                "Mods.sorceryFight.PassiveTechniques.SummonGaruda.AdditionalStats")
                .WithFormatArgs(
                    BOSS_MULTIPLIER
                ).Value;
            return baseStats + "\n" + additionalStats;
        }
    }
}
