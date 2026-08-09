using System.Collections.Generic;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.SFPlayer;
using sorceryFight.Content.InnateTechniques;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Chat;
using sorceryFight.Content.Particles.UIParticles;
using sorceryFight.Content.Particles;
using Terraria.Audio;
using Terraria.Localization;
using System.Linq;
using sorceryFight.Packets;

namespace sorceryFight.Content.InnateTechniques
{
    public abstract class InnateTechnique
    {
        /// <summary>
        /// The internal name of the innate technique
        /// </summary>
        public abstract string InternalName { get; }


        /// <summary>
        /// The display name of the innate technique
        /// </summary>
        public string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.Misc.InnateTechniques.{InternalName}.DisplayName");
        public abstract List<PassiveTechnique> PassiveTechniques { get; }
        public abstract List<CursedTechnique> CursedTechniques { get; }
        public abstract PlayerDomainExpansion DomainExpansion { get; }

        public virtual Color innateBGColor { get; set; }
        public virtual Color innateBorderColor { get; set; }

        internal int rctTimer = 0;

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
                case "BloodManipulation":
                    return new BloodManipulationTechnique();
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
                    new BloodManipulationTechnique(),
                    new PrivatePureLoveTrainTechnique(),
                    new HeavenlyRestriction(),
                };
            }
        }


        public virtual void Initialize(SorceryFightPlayer sf)
        {
            sf.onRevive += () => { sf.rctAnimation = true; };
        }

        /// <summary>
        /// Used for technique-specific modifications to class damage, defense, speed, etc.
        /// </summary>
        public virtual void UpdateEquips(SorceryFightPlayer sf) { }

        /// <summary>
        /// Used for technique-specific modifications heath regeneration.
        /// </summary>
        public virtual void UpdateLifeRegen(SorceryFightPlayer sf) { }

        public virtual void PreUpdate(SorceryFightPlayer sf)
        {
            if (sf.rctAnimation)
                RCTAnimation(sf);
        }


        /// <summary>
        /// Base method that allows each technique to have their own reverse cursed technique
        /// unlocking animation. Defaults to legacy animation.
        /// </summary>
        public virtual void RCTAnimation(SorceryFightPlayer sf)
        {
            SetupRCTAnimation(sf);

            if (rctTimer % 90 == 0)
            {
                SoundEngine.PlaySound(SorceryFightSounds.CommonHeartBeat with { Volume = 2f }, sf.Player.Center);
            }

            int numParticles = rctTimer / 90;
            for (int i = 0; i <= numParticles; i++)
            {
                Vector2 particlePosition = sf.Player.Center + new Vector2(Main.rand.NextFloat(-100f, 100f), Main.rand.NextFloat(-100f, 100f));
                Vector2 particleVelocity = particlePosition.DirectionTo(sf.Player.Center) * 3;
                LinearParticle particle = new LinearParticle(particlePosition, particleVelocity, Color.Wheat, false, 0.9f, 0.5f, 30);
                ParticleController.SpawnParticle(particle);
            }

            if (rctTimer >= 300)
            {
                rctTimer = 0;
                GrantRCT(sf);

                if (sf.heavenlyRestriction)
                {
                    ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText("Mods.sorceryFight.Misc.LeftItAllBehind.GeneralMessage"), Color.Green, sf.Player.whoAmI);
                }
                else
                {
                    string keybindText = "[" + SFKeybinds.UseRCT.GetAssignedKeys()[sf.Player.whoAmI] + "]" + SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.UnlockedRCT.KeyBindMessage");
                    ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText("Mods.sorceryFight.Misc.UnlockedRCT.GeneralMessage"), Color.Green, sf.Player.whoAmI);
                    ChatHelper.SendChatMessageToClient(NetworkText.FromLiteral(keybindText), Color.Green, sf.Player.whoAmI);
                }


                for (int i = 0; i < 100; i++)
                {
                    Vector2 particleOffsetPosition = sf.Player.Center + new Vector2(Main.rand.NextFloat(-200f, 200f), Main.rand.NextFloat(-200f, 200f));
                    Vector2 particleVelocity = sf.Player.Center.DirectionTo(particleOffsetPosition) * 6;
                    LinearParticle particle = new LinearParticle(sf.Player.Center, particleVelocity, Color.Wheat, false, 0.9f, 2f, 90);
                    ParticleController.SpawnParticle(particle);
                }
            }
        }


        internal void SetupRCTAnimation(SorceryFightPlayer sf)
        {
            rctTimer++;
            sf.Player.creativeGodMode = true;
            sf.Player.immune = true;
            sf.Player.immuneTime = 60;

            if (sf.Player.statLife < sf.Player.statLifeMax2)
            {
                sf.Player.statLife++;
            }

            if (sf.deathPosition == Vector2.Zero)
            {
                sf.deathPosition = sf.Player.position;
            }
            sf.Player.position = sf.deathPosition;
        }


        internal void GrantRCT(SorceryFightPlayer sf)
        {
            rctTimer = 0;
            sf.Player.creativeGodMode = false;
            sf.rctAnimation = false;
            sf.Player.immune = false;
            sf.deathPosition = Vector2.Zero;
            sf.unlockedRCT = true;
            SorceryFightUI.UpdateTechniqueUI.Invoke();
        }




        public InnateTechnique()
        {
            foreach (CursedTechnique ct in CursedTechniques)
            {
                ct.SetParentTechnique(InternalName);
            }
        }
    }
}
