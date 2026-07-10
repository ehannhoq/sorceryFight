using sorceryFight.SFPlayer;
using Terraria;


namespace sorceryFight.Content.Buffs.BloodManipulation
{
    public class FlowingRedScale : PassiveTechnique
    {
        public override string InternalName => "FlowingRedScale";

        public FlowingRedScale()
        {
            Technique.cost = 10;
        }

        private const float BOSS_MULITIPLIER = 1.5f;
        private const int DEFENSE_ADDITION = 12;
        private const float DAMAGE_NEGATION = 0.10f;
        private const int BLOOD_REGEN_PER_SECOND = 10;

        public override void OnApply(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.disableCurseTechniques = true;
        }

        public override void OnRemove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.disableCurseTechniques = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            player.endurance += DAMAGE_NEGATION;
            player.statDefense += DEFENSE_ADDITION;

            float multiplier = 1;
            if (AreThereAnyDamnBosses.BossActive)
            {
                multiplier = BOSS_MULITIPLIER;
            }

            if (sfPlayer.unlockedRCT)
            {
                Technique.cost = 10f;
                Technique.cost += Technique.cost * multiplier;
            }
            else
            {
                Technique.cost = 0;
                player.lifeRegen -= 10;
            }

            Technique.cost += Technique.cost * multiplier;

            base.Update(player, ref buffIndex);
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string baseStats =  base.GetStats(sf);
            string additionalStats = SFUtils.GetLocalization(
                "Mods.sorceryFight.PassiveTechniques.FlowingRedScale.AdditionalStats")
                .WithFormatArgs(
                    DEFENSE_ADDITION,
                    DAMAGE_NEGATION,
                    BLOOD_REGEN_PER_SECOND,
                    BOSS_MULITIPLIER
                ).Value;
            return baseStats + "\n" + additionalStats;
        }

    }
}
