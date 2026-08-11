using System;
using System.Collections.Generic;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.TenShadows
{
    public class Totality : PassiveTechnique
    {
        public override string InternalName => "Totality";

        public Totality()
        {
            Technique.cost = 10;
        }

        public override void OnApply(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.TotalityToggle = true;
        }

        public override void OnRemove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.TotalityToggle = false;

        }
    }
}
