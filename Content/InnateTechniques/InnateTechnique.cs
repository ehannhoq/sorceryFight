using System;
using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.Content.DomainExpansions;

namespace sorceryFight.Content.InnateTechniques
{
    public abstract class InnateTechnique()
    {
        public abstract string Name { get; }
        public abstract string DisplayName { get; }
        public abstract List<PassiveTechnique> PassiveTechniques { get; }
        public abstract List<CursedTechnique> CursedTechniques { get; }
        public abstract DomainExpansion DomainExpansion { get; }
        public abstract bool IsValid { get; }

        public static InnateTechnique GetInnateTechnique(string name)
        {
            switch (name)
            {
                case "Limitless":
                    return new LimitlessTechnique();
            }

            return null;
        }

        public static List<InnateTechnique> InnateTechniques
        {
            get
            {
                return new List<InnateTechnique>
                {
                    new LimitlessTechnique()
                };
            }
        }

        public virtual void PostUpdate(SorceryFightPlayer sf) {}

        public virtual void ExpandDomain(SorceryFightPlayer sf) { }
        public virtual void CloseDomain(SorceryFightPlayer sf)
        {
            Main.npc[sf.domainIndex].active = false;
            sf.Player.AddBuff(ModContent.BuffType<BurntTechnique>(), SorceryFight.BuffSecondsToTicks(210));
            sf.expandedDomain = false;
            sf.disableRegenFromDE = false;
            sf.domainIndex = -1;
        }
    }
}