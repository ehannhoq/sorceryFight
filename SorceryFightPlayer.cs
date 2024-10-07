using System;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.PassiveTechniques.Limitless;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{
    public class SorceryFightPlayer : ModPlayer
    {
        #region Global Cursed Technique Stuff
        public string innateTechnique = "No Innate Technique";
        public float mastery = 0f;
        public float masteryMax = 100f;
        public float cursedEnergy = 100f;
        public float maxCursedEnergy = 100f;
        public float cursedEnergyRegenRate = 0.1f; // The regen rate per tick

        #endregion

        #region Passive Techniques
        public bool hasInfinity = false;
        #endregion

        #region Cursed Energy Modifiers
        #endregion

        
        public override void SaveData(TagCompound tag)
        {
            tag["innateTechnique"] = innateTechnique;
            tag["mastery"] = mastery;
            tag["cursedEnergy"] = cursedEnergy;
        }

        public override void LoadData(TagCompound tag)
        {
            innateTechnique = tag.ContainsKey("innateTechnique") ? tag.GetString("innateTechnique") : "";
            mastery = tag.ContainsKey("mastery") ? tag.GetFloat("mastery") : 0f;
            cursedEnergy = tag.ContainsKey("cursedEnergy") ? tag.GetFloat("cursedEnergy") : 0f;

            cursedEnergyRegenRate = calculateCERegenRate();
            maxCursedEnergy = calculateMaxCE(); // Will calculate at the very end, to ensure all buffs are accounted for.
        }

        public override void PostUpdate()
        {
            if (cursedEnergy < maxCursedEnergy)
            {
                cursedEnergy += cursedEnergyRegenRate;
                if (cursedEnergy > maxCursedEnergy)
                {
                    cursedEnergy = maxCursedEnergy;
                }
            }

            if (cursedEnergy < 0)
            {
                cursedEnergy = 0;
                removeBuffs();
            }
        }

        public override void PostUpdateBuffs()
        {
            if (hasInfinity)
                Player.AddBuff(ModContent.BuffType<Infinity>(), 2);
            Player.creativeGodMode = hasInfinity;

        }

        public override void UpdateDead()
        {
            hasInfinity = false;
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
            float baseRegen = 1f;

            return baseRegen + sum;
        }

        private void removeBuffs()
        {
            hasInfinity = false;
        }

    }
}