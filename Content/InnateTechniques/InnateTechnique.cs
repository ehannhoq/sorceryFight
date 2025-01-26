using System;
using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.SFPlayer;

namespace sorceryFight.Content.InnateTechniques
{
    public abstract class InnateTechnique()
    {
        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract List<PassiveTechnique> PassiveTechniques { get; }
        public abstract List<CursedTechnique> CursedTechniques { get; }
        public abstract DomainExpansion DomainExpansion { get; }
        public virtual int DomainExpansionTimer { get; set; } = -1;

        public static InnateTechnique GetInnateTechnique(string name)
        {
            switch (name)
            {
                case "Limitless":
                    return new LimitlessTechnique();
                case "Shrine":
                    return new ShrineTechnique();
            }

            return null;
        }

        public static List<InnateTechnique> InnateTechniques
        {
            get
            {
                return new List<InnateTechnique>
                {
                    new LimitlessTechnique(),
                    new ShrineTechnique()
                };
            }
        }

        public virtual void PostUpdate(SorceryFightPlayer sf) {}

        public virtual void ExpandDomain(SorceryFightPlayer sf) { }
        public virtual void CloseDomain(SorceryFightPlayer sf)
        {
            Main.npc[sf.domainIndex].active = false;
            sf.AddBurntTechniqueDebuff(SorceryFightPlayer.DefaultBurntTechniqueDurationFromDE);
            sf.expandedDomain = false;
            sf.disableRegenFromDE = false;
            sf.domainIndex = -1;
        }
    }
}