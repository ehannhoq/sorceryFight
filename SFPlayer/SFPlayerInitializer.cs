using System;
using System.Collections.Generic;
using System.Linq;
using sorceryFight.Content.InnateTechniques;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public override void Initialize()
        {
            disableRegenFromProjectiles = false;
            disableRegenFromBuffs = false;
            disableRegenFromDE = false;
            ctCostReduction = 0f;

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

            recievedYourPotential = false;
            yourPotentialSwitch = false;
            usedYourPotentialBefore = false;
            usedCursedFists = false;
            npcsHitWithCursedFists = new HashSet<int>();
            idleDeathGambleBuffStrength = 0;

            inDomainAnimation = false;
            domainTimer = 0;
            fallingBlossomEmotion = false;
            inSimpleDomain = false;

            sixEyes = false;
            challengersEye = false;
            uniqueBodyStructure = false;
            blessedByBlackFlash = false;

            explosiveCursedEnergy = false;
            sharpCursedEnergy = false;
            overflowingEnergy = false;

            sukunasFingers = new bool[20];

            unlockedRCT = false;
            rctAuraIndex = -1;
            rctBaseHealPerSecond = 60;
            additionalRCTHealPerSecond = 0;
            rctEfficiency = 0.0f;

            celestialAmulet = false;
            pictureLocket = false;
            cursedOfuda = false;

            blackFlashDamageMultiplier = 3;
            blackFlashTime = 30;
            lowerWindowTime = 15;
            blackFlashWindowTime = 1;
            blackFlashTimeLeft = -60;
            blackFlashCounter = 0;
            additionalBlackFlashDamageMultiplier = 0f;
        }
        public override void SaveData(TagCompound tag)
        {
            tag["ctCostReduction"] = ctCostReduction;

            if (innateTechnique != null)
                tag["innateTechnique"] = innateTechnique.Name;

            tag["cursedEnergy"] = cursedEnergy;

            tag["bossesDefeated"] = bossesDefeated;
            
            tag["ctSelector"] = new List<float> { CTSelectorPos.X, CTSelectorPos.Y };

            tag["ptSelector"] = new List<float> { PTSelectorPos.X, PTSelectorPos.Y };

            tag["ceBar"] = new List<float> { CEBarPos.X, CEBarPos.Y };

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
            generalBooleans.AddWithCondition("recievedYourPotential", recievedYourPotential);
            generalBooleans.AddWithCondition("usedYourPotentialBefore", usedYourPotentialBefore);
            generalBooleans.AddWithCondition("unlockedRCT", unlockedRCT);
            generalBooleans.AddWithCondition("sixEyes", sixEyes);
            generalBooleans.AddWithCondition("hollowEyes", challengersEye);
            generalBooleans.AddWithCondition("uniqueBodyStructure", uniqueBodyStructure);
            generalBooleans.AddWithCondition("blessedByBlackFlash", blessedByBlackFlash);
            tag["generalBooleans"] = generalBooleans;

            if (innateTechnique != null)
            {
                var indexes = new List<int>();
                for (int i = 0; i < 20; i++)
                {
                    if (sukunasFingers[i])
                        indexes.Add(i);
                }
                tag["sukunasFingers"] = indexes;
            }

        }

        public override void LoadData(TagCompound tag)
        {
            ctCostReduction = tag.ContainsKey("ctCostReduction") ? tag.GetFloat("ctCostReduction") : 0f;

            string innateTechniqueName = tag.ContainsKey("innateTechnique") ? tag.GetString("innateTechnique") : "";
            innateTechnique = InnateTechnique.GetInnateTechnique(innateTechniqueName);

            cursedEnergy = tag.ContainsKey("cursedEnergy") ? tag.GetFloat("cursedEnergy") : 1f;
            var defeatedBosses = tag.ContainsKey("bossesDefeated") ? tag.GetList<int>("bossesDefeated") : new List<int>();
            bossesDefeated = defeatedBosses as List<int>;

            CTSelectorPos = tag.ContainsKey("ctSelector")
            ? new Microsoft.Xna.Framework.Vector2(tag.Get<List<float>>("ctSelector")[0], tag.Get<List<float>>("ctSelector")[1])
            : Microsoft.Xna.Framework.Vector2.Zero;

            PTSelectorPos = tag.ContainsKey("ptSelector")
            ? new Microsoft.Xna.Framework.Vector2(tag.Get<List<float>>("ptSelector")[0], tag.Get<List<float>>("ptSelector")[1])
            : Microsoft.Xna.Framework.Vector2.Zero;

            CEBarPos = tag.ContainsKey("ceBar")
            ? new Microsoft.Xna.Framework.Vector2(tag.Get<List<float>>("ceBar")[0], tag.Get<List<float>>("ceBar")[1])
            : Microsoft.Xna.Framework.Vector2.Zero;

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
            recievedYourPotential = generalBooleans.Contains("recievedYourPotential");
            usedYourPotentialBefore = generalBooleans.Contains("usedYourPotentialBefore");
            unlockedRCT = generalBooleans.Contains("unlockedRCT");
            sixEyes = generalBooleans.Contains("sixEyes");
            challengersEye = generalBooleans.Contains("hollowEyes");
            uniqueBodyStructure = generalBooleans.Contains("uniqueBodyStructure");
            blessedByBlackFlash = generalBooleans.Contains("blessedByBlackFlash");

            maxCursedEnergy = calculateBaseMaxCE();
            cursedEnergyRegenPerSecond = calculateBaseCERegenRate();

            if (innateTechnique != null)
            {
                var indexes = tag.GetList<int>("sukunasFingers");
                
                for (int i = 0; i < indexes.Count; i++)
                {
                    sukunasFingers[indexes[i]] = true;
                }
            }
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
