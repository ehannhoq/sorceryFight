using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.Buffs;
using Terraria;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using System.IO;
using System;
using sorceryFight.Content.InnateTechniques;

namespace sorceryFight.Content.InnateTechniques
{
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
    public abstract PlayerDomainExpansion DomainExpansion { get; }

    public static InnateTechnique GetInnateTechnique(string name)
    {
        switch (name)
        {
            case "Limitless":
                return new LimitlessTechnique();
            case "Shrine":
                return new ShrineTechnique();
            case "Vessel":
                return new VesselTechnique();
            case "PrivatePureLoveTrain":
                return new PrivatePureLoveTrainTechnique();
            case "HeavenlyRestriction":
                return new HeavenlyRestriction();
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
                    new ShrineTechnique(),
                    new VesselTechnique(),
                    new PrivatePureLoveTrainTechnique(),
                    new HeavenlyRestriction()
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
