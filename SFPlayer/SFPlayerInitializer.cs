using System;
using System.Collections.Generic;
using sorceryFight.Content.InnateTechniques;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public override void Initialize()
        {
            hasUIOpen = false;
            disableRegenFromProjectiles = false;
            disableRegenFromBuffs = false;
            disableRegenFromDE = false;

            innateTechnique = null;
            selectedTechnique = null;
            cursedEnergy = 0f;
            maxCursedEnergy = 100f;
            cursedEnergyRegenPerSecond = 1f;
            cursedEnergyUsagePerSecond = 0f;

            bossesDefeated = new List<int>();

            cursedSkull = false;
            cursedMechanicalSoul = false;
            cursedPhantasmalEye = false;
            cursedProfaneShards = false;
            maxCursedEnergyFromOtherSources = 0f;

            cursedEye = false;
            cursedFlesh = false;
            cursedBulb = false;
            cursedMask = false;
            cursedEffulgentFeather = false;
            cursedRuneOfKos = false;
            cursedEnergyRegenFromOtherSources = 0f;

            yourPotentialSwitch = false;
            usedYourPotentialBefore = false;

            expandedDomain = false;
            domainIndex = -1;

            sixEyes = false;
            uniqueBodyStructure = false;
            sukunasVessel = false;

            unlockedRCT = false;
            rctAuraIndex = -1;

            celestialAmulet = false;
        }
        public override void SaveData(TagCompound tag)
        {
            if (innateTechnique != null)
                tag["innateTechnique"] = innateTechnique.Name;

            tag["cursedEnergy"] = cursedEnergy;

            tag["bossesDefeated"] = bossesDefeated;

            var maxCEModifiers = new List<string>();
            maxCEModifiers.AddWithCondition("cursedSkull", cursedSkull);
            maxCEModifiers.AddWithCondition("cursedMechanicalSoul", cursedMechanicalSoul);
            maxCEModifiers.AddWithCondition("cursedPhantasmalEye", cursedPhantasmalEye);
            maxCEModifiers.AddWithCondition("cursedProfanedShards", cursedProfaneShards);
            tag["maxCEModifiers"] = maxCEModifiers;


            var cursedEnergyRegenModifiers = new List<string>();
            cursedEnergyRegenModifiers.AddWithCondition("cursedEye", cursedEye);
            cursedEnergyRegenModifiers.AddWithCondition("cursedFlesh", cursedFlesh);
            cursedEnergyRegenModifiers.AddWithCondition("cursedBulb", cursedBulb);
            cursedEnergyRegenModifiers.AddWithCondition("cursedMask", cursedMask);
            cursedEnergyRegenModifiers.AddWithCondition("cursedEffulgentFeather", cursedEffulgentFeather);
            cursedEnergyRegenModifiers.AddWithCondition("cursedRuneOfKos", cursedRuneOfKos);
            tag["cursedEnergyRegenModifiers"] = cursedEnergyRegenModifiers;

            var generalBooleans = new List<string>();
            generalBooleans.AddWithCondition("usedYourPotentialBefore", usedYourPotentialBefore);
            generalBooleans.AddWithCondition("unlockedRCT", unlockedRCT);
            generalBooleans.AddWithCondition("sixEyes", sixEyes);
            generalBooleans.AddWithCondition("uniqueBodyStructure", uniqueBodyStructure);
            generalBooleans.AddWithCondition("sukunasVessel", sukunasVessel);
            tag["generalBooleans"] = generalBooleans;

        }

        public override void LoadData(TagCompound tag)
        {
            string innateTechniqueName = tag.ContainsKey("innateTechnique") ? tag.GetString("innateTechnique") : "";
            innateTechnique = InnateTechnique.GetInnateTechnique(innateTechniqueName);

            cursedEnergy = tag.ContainsKey("cursedEnergy") ? tag.GetFloat("cursedEnergy") : 1f;
            var defeatedBosses = tag.ContainsKey("bossesDefeated") ? tag.GetList<int>("bossesDefeated") : new List<int>();
            bossesDefeated = defeatedBosses as List<int>;

            var maxCEModifiers = tag.GetList<string>("maxCEModifiers");
            cursedSkull = maxCEModifiers.Contains("cursedSkull");
            cursedMechanicalSoul = maxCEModifiers.Contains("cursedMechanicalSoul");
            cursedPhantasmalEye = maxCEModifiers.Contains("cursedPhantasmalEye");
            cursedProfaneShards = maxCEModifiers.Contains("cursedProfanedShards");

            var cursedEnergyRegenModifiers = tag.GetList<string>("cursedEnergyRegenModifiers");
            cursedEye = cursedEnergyRegenModifiers.Contains("cursedEye");
            cursedFlesh = cursedEnergyRegenModifiers.Contains("cursedFlesh");
            cursedBulb = cursedEnergyRegenModifiers.Contains("cursedBulb");
            cursedMask = cursedEnergyRegenModifiers.Contains("cursedMask");
            cursedEffulgentFeather = cursedEnergyRegenModifiers.Contains("cursedEffulgentFeather");
            cursedRuneOfKos = cursedEnergyRegenModifiers.Contains("cursedRuneOfKos");

            var generalBooleans = tag.GetList<string>("generalBooleans");
            usedYourPotentialBefore = generalBooleans.Contains("usedYourPotentialBefore");
            unlockedRCT = generalBooleans.Contains("unlockedRCT");
            sixEyes = generalBooleans.Contains("sixEyes");
            uniqueBodyStructure = generalBooleans.Contains("uniqueBodyStructure");
            sukunasVessel = generalBooleans.Contains("sukunasVessel");

            maxCursedEnergy = calculateBaseMaxCE();
            cursedEnergyRegenPerSecond = calculateBaseCERegenRate();
        }

        public float calculateBaseMaxCE()
        {
            float baseCE = 100f;
            float sum = 0f;

            if (cursedSkull)
                sum += 100f; // 200 total

            if (cursedMechanicalSoul)
                sum += 300f; // 500 total

            if (cursedPhantasmalEye)
                sum += 500f; // 1000 total

            if (cursedProfaneShards)
                sum += 1000f; // 2000 total

            return baseCE + sum;
        }

        public float calculateBaseCERegenRate()
        {
            float baseRegen = 1f;
            float sum = 0f;

            if (cursedEye)
                sum += 4f; // 5 CE/s

            if (cursedFlesh)
                sum += 10f; // 15 CE/s total

            if (cursedBulb)
                sum += 15f; // 30 CE/s total

            if (cursedMask)
                sum += 20f; // 50 CE/s total

            if (cursedEffulgentFeather)
                sum += 25f; // 75 CE/s

            if (cursedRuneOfKos)
                sum += 25f; // 100 CE/s total

            return baseRegen + sum;
        }
    }
}
