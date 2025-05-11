using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using sorceryFight.SFPlayer;
using sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain;

namespace sorceryFight.Content.InnateTechniques
{
    public class PrivatePureLoveTrainTechnique : InnateTechnique
    {
        public override string Name => "PrivatePureLoveTrain";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.PrivatePureLoveTrain.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {

        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new PachinkoBalls(),
            new HakarisDoor()
        };

        public override DomainExpansion DomainExpansion { get; } = new IdleDeathGamble();
    }
}