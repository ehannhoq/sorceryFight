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
        
        public override void PreUpdate(SorceryFightPlayer sf)
        {
            if (DomainExpansionTimer == -1)
            {
                return;
            }

            DomainExpansionTimer ++;

            if (DomainExpansionTimer == 1)
            {
                int index = CombatText.NewText(sf.Player.getRect(), Color.White, "Domain Expansion:");
                Main.combatText[index].lifeTime = 90;
            }

            if (DomainExpansionTimer == 101)
            {
                int index = CombatText.NewText(sf.Player.getRect(), Color.White, "Idle Death Gamble");
                Main.combatText[index].lifeTime = 90;
            }

            if (DomainExpansionTimer == 91)
            {
                SoundEngine.PlaySound(SorceryFightSounds.IdleDeathGambleOpening with { Volume = 0.75f }, sf.Player.Center);
            }
            
            if (DomainExpansionTimer == 211)
            {
                Terraria.DataStructures.IEntitySource entitySource = sf.Player.GetSource_FromThis();
                Vector2 position = sf.Player.Center;

                if (Main.myPlayer == sf.Player.whoAmI)
                    sf.domainIndex = NPC.NewNPC(entitySource, (int)position.X, (int)position.Y, ModContent.NPCType<IdleDeathGamble>(), 0, default, sf.Player.whoAmI);
                DomainExpansionTimer = -1;
            }

        }
    }
}