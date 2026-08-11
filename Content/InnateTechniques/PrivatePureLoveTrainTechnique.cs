using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using Microsoft.Xna.Framework;
using sorceryFight.Utilities;
using Terraria.ID;
using sorceryFight.SFPlayer;

namespace sorceryFight.Content.InnateTechniques
{
    public class PrivatePureLoveTrainTechnique : InnateTechnique
    {
        public override string InternalName => "PrivatePureLoveTrain";

        public override Color innateBGColor => new Color(160, 232, 64, 85);

        public override Color innateBorderColor => new Color(0, 0, 0, 128);
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {

        };
        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new PachinkoBalls()
                .SetUnlock(NPCID.EyeofCthulhu),

            new HakarisDoor()
                .SetUnlock(NPCID.SkeletronHead),

            new PassingThrough()
                .SetUnlock(NPCID.WallofFlesh),

            new CargoCrate()
                .SetUnlock((SorceryFightPlayer sfPlayer) => sfPlayer.defeatedMechBossThree)
                .SetUnlockRequirement("Mods.sorceryFight.UnlockRequirements.MechBossThree"),

            new RailroadSign()
                .SetUnlock(NPCID.HallowBoss)
        };

        public override PlayerDomainExpansion DomainExpansion { get; } = new IdleDeathGamble();
    }
}