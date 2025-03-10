using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.Buffs.Shrine;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.CursedTechniques.Shrine;
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
        public override string DisplayName => SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.InnateTechniques.Shrine.DisplayName");
        public override List<PassiveTechnique> PassiveTechniques { get; } = new List<PassiveTechnique>
        {
            new DomainAmplificationBuff(),
            new HollowWickerBasketBuff()
        };

        public override List<CursedTechnique> CursedTechniques { get; } = new List<CursedTechnique>
        {
            new Dismantle(),
            new Cleave(),
            new InstantDismantle(),
            new DivineFlame(),
            new WorldCuttingSlash()
        };

        public override DomainExpansion DomainExpansion { get; } = new MalevolentShrine();

        public override void PostUpdate(SorceryFightPlayer sf)
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

            if (DomainExpansionTimer == 47)
            {
                SoundEngine.PlaySound(SorceryFightSounds.MalevolentShrine, sf.Player.Center);
            }
            
            if (DomainExpansionTimer == 211)
            {
                Terraria.DataStructures.IEntitySource entitySource = sf.Player.GetSource_FromThis();
                Vector2 position = sf.Player.Center;

                if (Main.myPlayer == sf.Player.whoAmI)
                    sf.domainIndex = NPC.NewNPC(entitySource, (int)position.X, (int)position.Y, ModContent.NPCType<MalevolentShrine>(), 0, default, sf.Player.whoAmI);
                DomainExpansionTimer = -1;
            }
        }

        public override void ExpandDomain(SorceryFightPlayer sf)
        {
            DomainExpansionTimer = 0;
        }
    }
}
