using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.PassiveTechniques;
using sorceryFight.Content.PassiveTechniques.Limitless;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{
    public class SorceryFightPlayer : ModPlayer
    {
        #region Global Variables
        public bool hasUIOpen = false;
        #endregion

        #region Global Cursed Technique Stuff
        public InnateTechnique innateTechnique = new InnateTechnique();
        public CursedTechnique selectedTechnique = new CursedTechnique();
        public float mastery = 0f;
        public float cursedEnergy = 100f;
        public float maxCursedEnergy = 100f;
        public float cursedEnergyRegenPerSecond;

        #endregion

        #region Cursed Energy Modifiers
        #endregion

        public override void SaveData(TagCompound tag)
        {
            tag["innateTechnique"] = innateTechnique.Name;
            tag["mastery"] = mastery;
            tag["cursedEnergy"] = cursedEnergy;
        }

        public override void LoadData(TagCompound tag)
        {
            string innateTechniqueName = tag.ContainsKey("innateTechnique") ? tag.GetString("innateTechnique") : "";
            innateTechnique = InnateTechnique.GetInnateTechnique(innateTechniqueName);
            
            mastery = tag.ContainsKey("mastery") ? tag.GetFloat("mastery") : 0f;
            cursedEnergy = tag.ContainsKey("cursedEnergy") ? tag.GetFloat("cursedEnergy") : 0f;

            cursedEnergyRegenPerSecond = calculateCERegenRate();
            maxCursedEnergy = calculateMaxCE(); // Will calculate at the very end, to ensure all buffs are accounted for.
        }

        public override void PostUpdate()
        {
            if (cursedEnergy < maxCursedEnergy)
            {
                cursedEnergy += SorceryFight.SecondsToTicks(cursedEnergyRegenPerSecond);
                if (cursedEnergy > maxCursedEnergy)
                {
                    cursedEnergy = maxCursedEnergy;
                }
            }

            if (cursedEnergy < 0)
            {
                cursedEnergy = 0;
            }

            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                if (cursedEnergy <= 0)
                {
                    passiveTechnique.isActive = false;
                }

                if (passiveTechnique.isActive)
                {
                    passiveTechnique.Apply(Player);
                }
                else
                {
                    passiveTechnique.Remove(Player);
                }
            }
        }

        public override void UpdateDead()
        {
            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                passiveTechnique.isActive = false;
            }
        }

        private float calculateMaxCE()
        {
            float sum = 0f;
            float baseCE = 100f;

            // This is where all the conditions will go for modifiers.

            return baseCE + sum;
        }

        private float calculateCERegenRate()
        {
            float sum = 0f;
            float baseRegen = 10f;
            
            // This is where all the conditions will go for modifiers.

            return baseRegen + sum;
        }
    }
}