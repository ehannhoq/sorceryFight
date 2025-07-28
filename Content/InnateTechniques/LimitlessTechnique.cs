using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.DomainExpansions.PlayerDomains;

namespace sorceryFight.Content.InnateTechniques
{
    public class LimitlessTechnique : InnateTechnique
    {
        public override string Name => "Limitless";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Limitless.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new InfinityBuff(),
            new AmplifiedAuraBuff(),
            new MaximumAmplifiedAuraBuff(),
            new FallingBlossomEmotionBuff()
        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new AmplificationBlue(),
            new MaximumOutputBlue(),

            new ReversalRed(),
            new MaximumOutputRed(),
        
            new HollowPurple(),
            new HollowPurple200Percent()
        };

        public override PlayerDomainExpansion DomainExpansion => new UnlimitedVoid();
    }
}