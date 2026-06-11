using System.Collections.Generic;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.IceFormation;
//using sorceryFight.Content.Buffs.StarRage;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques.TenShadows;
using sorceryFight.Utilities;
using sorceryFight.Content.Buffs.TenShadows;

namespace sorceryFight.Content.InnateTechniques
{
    public class TenShadowsTechnique : InnateTechnique
    {
        public override string InternalName => "TenShadows";

        public override Color innateBGColor => new Color(11, 13, 30, 70);

        public override Color innateBorderColor => new Color(18, 61, 116, 128);

        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new TotalityBuff()
                .SetUnlock((SorceryFightPlayer sfPlayer) => true)
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            //divine white spawns black as a child / becomes totality
            new DivineWhite()
                .SetUnlock((SorceryFightPlayer sfPlayer) => true),
            new Nue()
                .SetUnlock((SorceryFightPlayer sfPlayer) => true),
            new Toad()
                .SetUnlock((SorceryFightPlayer sfPlayer) => true),
            new MaxElephant()
                .SetUnlock((SorceryFightPlayer sfPlayer) => true),
            new RoundDeer()
                .SetUnlock((SorceryFightPlayer sfPlayer) => true)
            

        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new HeavensRime();


    }
}