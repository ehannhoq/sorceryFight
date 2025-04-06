using System.Collections.Generic;
using CalamityMod;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using sorceryFight.Content.CursedTechniques.Vessel;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight.Content.InnateTechniques
{
    public class VesselTechnique : InnateTechnique
    {
        public override string Name => "Vessel";
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Vessel.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new SoulDismantle(),
            new ChainDismantle(),
            new PiercingBlood()
        };

        public override DomainExpansion DomainExpansion { get; } = new Home();

        public override void UpdateEquips(SorceryFightPlayer sf)
        {
            sf.Player.GetDamage(DamageClass.Melee) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Ranged) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Magic) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(DamageClass.Summon) *= 1 + (0.05f * sf.sukunasFingerConsumed);
            sf.Player.GetDamage(RogueDamageClass.Throwing) *= 1 + (0.05f * sf.sukunasFingerConsumed);

            sf.Player.statDefense *= 1 + (0.03f * sf.sukunasFingerConsumed);
        }

        public override void UpdateLifeRegen(SorceryFightPlayer sf) 
        {
            sf.Player.lifeRegen += 2 * sf.sukunasFingerConsumed;
        }


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
                int index = CombatText.NewText(sf.Player.getRect(), Color.White, "Home");
                Main.combatText[index].lifeTime = 90;
            }

            if (DomainExpansionTimer == 91)
            {
                SoundEngine.PlaySound(SorceryFightSounds.IdleDeathGambleOpening, sf.Player.Center);
            }
            
            if (DomainExpansionTimer == 211)
            {
                Terraria.DataStructures.IEntitySource entitySource = sf.Player.GetSource_FromThis();
                Vector2 position = sf.Player.Center;

                if (Main.myPlayer == sf.Player.whoAmI)
                    sf.domainIndex = NPC.NewNPC(entitySource, (int)position.X, (int)position.Y, ModContent.NPCType<Home>(), 0, default, sf.Player.whoAmI);
                DomainExpansionTimer = -1;
            }
        }

        public override void ExpandDomain(SorceryFightPlayer sf)
        {
            DomainExpansionTimer = 0;
        }
    }
}
