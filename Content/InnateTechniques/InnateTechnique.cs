using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.Buffs;
using Terraria;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using System.IO;
using System;

namespace sorceryFight.Content.InnateTechniques
{
    public enum InnateTechniqueType : byte
    {
        None,
        Limitless,
        Shrine,
        Vessel,
        PPLT
    }
    public static class InnateTechniqueFactory
    {
        public static InnateTechnique Create(InnateTechniqueType type)
        {
            return type switch
            {
                InnateTechniqueType.Limitless => new LimitlessTechnique(),
                // InnateTechniqueType.Shrine => new ShrineTechnique(),
                // InnateTechniqueType.Vessel => new VesselTechnique(),
                // InnateTechniqueType.PPLT => new PrivatePureLoveTrainTechnique(),
                _ => null
            };
        }

        public static InnateTechniqueType GetInnateTechniqueType(InnateTechnique technique)
        {
            if (typeof(LimitlessTechnique) == technique.GetType())
                return InnateTechniqueType.Limitless;

            // if (typeof(ShrineTechnique) == technique.GetType())
            //     return InnateTechniqueType.Shrine;

            // if (typeof(VesselTechnique) == technique.GetType())
            //     return InnateTechniqueType.Shrine;

            // if (typeof(PrivatePureLoveTrainTechnique) == technique.GetType())
            //     return InnateTechniqueType.PPLT;

            return InnateTechniqueType.None;
        }
    }
    public abstract class InnateTechnique()
    {
        /// <summary>
        /// The internal name of the innate technique
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The display name of the innate technique
        /// </summary>
        public abstract string DisplayName { get; }
        public abstract List<PassiveTechnique> PassiveTechniques { get; }
        public abstract List<CursedTechnique> CursedTechniques { get; }
        public abstract DomainExpansion DomainExpansion { get; }
        
        public static InnateTechnique GetInnateTechnique(string name)
        {
            switch (name)
            {
                case "Limitless":
                    return new LimitlessTechnique();
                // case "Shrine":
                //     return new ShrineTechnique();
                // case "Vessel":
                //     return new VesselTechnique();
                // case "PrivatePureLoveTrain":
                //     return new PrivatePureLoveTrainTechnique();
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
                    // new ShrineTechnique(),
                    // new VesselTechnique(),
                    // new PrivatePureLoveTrainTechnique()
                };
            }
        }

        /// <summary>
        /// Used for technique-specific modifications to class damage, defense, speed, etc.
        /// </summary>
        public virtual void UpdateEquips(SorceryFightPlayer sf) { }
        
        /// <summary>
        /// Used for technique-specific modifications heath regeneration.
        /// </summary>
        public virtual void UpdateLifeRegen(SorceryFightPlayer sf) { }
        public virtual void PreUpdate(SorceryFightPlayer sf) { }
    }
}