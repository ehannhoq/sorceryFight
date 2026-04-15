using CalamityMod;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Animations;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.StarRage
{
    public class SummonGarudaBuff : PassiveTechnique
    {
        
        //code made using Calamity's Tenryu as an example, HEAVILY modified to fit PassiveTechnique structure
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.SummonGarudaBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.SummonGarudaBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.SummonGarudaBuff.LockedDescription");

        public int Damage => 20;

        public override string Stats
        {
            get
            {
                return "Base CE Consumption: 10 CE/s\n"
                        + "Summon the Shikigami Garuda\n"
                        + "3x More Expensive when a boss is active\n";
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 10f;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.Golem);
        }

        public override void Apply(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            if (sfPlayer.summonGaruda == true)
            {
                return;
            }
            Vector2 spawnPos = new Vector2(player.position.X - 200, player.position.Y - 200);
            Vector2 spawnPos2 = new Vector2(player.position.X + 200, player.position.Y - 200);
            player.AddBuff(ModContent.BuffType<SummonGarudaBuff>(), 2);

            SummonGaruda(ModContent.ProjectileType<GarudaHead>(), ModContent.ProjectileType<GarudaBody>(), ModContent.ProjectileType<GarudaTail>(), spawnPos, player, player.GetSource_FromThis(), 20, 0);




            sfPlayer.summonGaruda = true;
        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            //sfPlayer.summonGaruda = false;
            sfPlayer.summonGaruda = false;
        }

        public static void SummonGaruda(int headType, int bodyType, int tailType, Vector2 spawnPos, Player player, IEntitySource source, int damage, float knockback)
        {

            SorceryFightMod.Log.Info($"ASummoning Garuda");


            var head = Projectile.NewProjectileDirect(source, spawnPos, player.DirectionTo(Main.MouseWorld) * 3, headType, damage, knockback, player.whoAmI);
            var tail = Projectile.NewProjectileDirect(source, spawnPos, Vector2.Zero, tailType, damage, knockback, player.whoAmI);
            for (var i = 0; i < 20; i++)
            {
                var body = Projectile.NewProjectileDirect(source, spawnPos, Vector2.Zero, bodyType, damage, knockback, player.whoAmI);
            }
        }

        public override bool UseCondition(Player player)
        {
            return !player.HasBuff(ModContent.BuffType<GarudaCooldown>());
        }


        public override void Update(Player player, ref int buffIndex)
        {

            int multiplier = 1;
            if (CalamityMod.CalPlayer.CalamityPlayer.areThereAnyDamnBosses)
            {
                multiplier = 3;
            }

            CostPerSecond = 10f;
            CostPerSecond *= multiplier;

            base.Update(player, ref buffIndex);
        }
    }
}