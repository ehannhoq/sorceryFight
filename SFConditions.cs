using System;
using Terraria;
using Terraria.Localization;

namespace sorceryFight
{

    public static class SFConditions
    {
        private static Condition Create(string key, Func<bool> predicate)
        {
            return new Condition(
                Language.GetText($"Mods.sorceryFight.Condition.{key}"),
                predicate
            );
        }
        
        public static Condition DownedFingerBearerI => Create("DownedFingerBearerI", () => SorceryFightDownedBossSystem.downedFingerBearerI);
        public static Condition DownedFingerBearerII => Create("DownedFingerBearerII", () => SorceryFightDownedBossSystem.downedFingerBearerII);
    }
}
