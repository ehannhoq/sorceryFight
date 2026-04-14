using System.Collections.Generic;
using System.Linq;
using MonoMod.Cil;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Quests;
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

            bloodEnergy = 0f;
            maxBloodEnergy = 100f;
            bloodEnergyRegenPerSecond = 1f;
            bloodEnergyUsagePerSecond = 0f;

            bossesDefeated = new HashSet<int>();

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
            idleDeathGambleBuffStrength = 0;

            inDomainAnimation = false;
            domainTimer = 0;
            fallingBlossomEmotion = false;
            inSimpleDomain = false;

            sixEyesLevel = 0;
            challengersEye = false;
            uniqueBodyStructure = false;
            blessedByBlackFlash = false;
            sukunasSkull = false;

            explosiveCursedEnergy = false;
            sharpCursedEnergy = false;
            overflowingEnergy = false;

            sukunasFingers = new bool[20];
            deathPaintings = new bool[8];

            unlockedRCT = false;
            rctAuraIndex = -1;
            rctBaseHealPerSecond = 60;
            additionalRCTHealPerSecond = 0;
            rctEfficiency = 0.0f;

            celestialAmulet = false;
            pictureLocket = false;
            cursedOfuda = false;
            beerHat = false;
            cursedBlindfold = false;

            blackFlashDamageMultiplier = 3;
            blackFlashTime = 30;
            lowerWindowTime = 15;
            blackFlashWindowTime = 1;
            blackFlashTimeLeft = -60;
            blackFlashCounter = 0;
            additionalBlackFlashDamageMultiplier = 0f;

            questData = new();
            currentQuests = new List<Quest>();
            completedQuests = new List<string>();

            mechanicalBossesDefeatedFlags = 0b0000;
        }
        public override void SaveData(TagCompound tag)
        {
            tag["ctCostReduction"] = ctCostReduction;

            if (innateTechnique != null)
                tag["innateTechnique"] = innateTechnique.Name;

            tag["cursedEnergy"] = cursedEnergy;

            tag["bloodEnergy"] = bloodEnergy;

            tag["bossesDefeated"] = bossesDefeated.ToList();

            tag["ctSelector"] = new List<float> { CTSelectorPos.X, CTSelectorPos.Y };

            tag["ptSelector"] = new List<float> { PTSelectorPos.X, PTSelectorPos.Y };

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
            generalBooleans.AddWithCondition("hollowEyes", challengersEye);
            generalBooleans.AddWithCondition("uniqueBodyStructure", uniqueBodyStructure);
            generalBooleans.AddWithCondition("blessedByBlackFlash", blessedByBlackFlash);
            generalBooleans.AddWithCondition("explosiveCursedEnergy", explosiveCursedEnergy);
            generalBooleans.AddWithCondition("sharpCursedEnergy", sharpCursedEnergy);
            generalBooleans.AddWithCondition("overflowingEnergy", overflowingEnergy);
            generalBooleans.AddWithCondition("sukunasSkull", sukunasSkull);
            tag["generalBooleans"] = generalBooleans;

            tag["sixEyesLevel"] = sixEyesLevel;

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

            if (innateTechnique != null)
            {
                var indexes = new List<int>();
                for (int i = 0; i < 8; i++)
                {
                    if (deathPaintings[i])
                        indexes.Add(i);
                }
                tag["deathPaintings"] = indexes;
            }

            var currentQuestsData = new List<string>();
            foreach (Quest q in currentQuests)
            {
                currentQuestsData.Add(q.GetClass());
            }
            tag["currentQuests"] = currentQuestsData;
            tag["completedQuests"] = completedQuests;

            tag["mechanicalBossesDefeatedFlags"] = mechanicalBossesDefeatedFlags;
        }

        public override void LoadData(TagCompound tag)
        {
            ctCostReduction = tag.ContainsKey("ctCostReduction") ? tag.GetFloat("ctCostReduction") : 0f;

            string innateTechniqueName = tag.ContainsKey("innateTechnique") ? tag.GetString("innateTechnique") : "";
            innateTechnique = InnateTechnique.GetInnateTechnique(innateTechniqueName);

            cursedEnergy = tag.ContainsKey("cursedEnergy") ? tag.GetFloat("cursedEnergy") : 1f;
            var defeatedBosses = tag.ContainsKey("bossesDefeated") ? tag.GetList<int>("bossesDefeated") : new List<int>();
            bossesDefeated = defeatedBosses.ToHashSet();

            CTSelectorPos = tag.ContainsKey("ctSelector")
            ? new Microsoft.Xna.Framework.Vector2(tag.Get<List<float>>("ctSelector")[0], tag.Get<List<float>>("ctSelector")[1])
            : Microsoft.Xna.Framework.Vector2.Zero;

            PTSelectorPos = tag.ContainsKey("ptSelector")
            ? new Microsoft.Xna.Framework.Vector2(tag.Get<List<float>>("ptSelector")[0], tag.Get<List<float>>("ptSelector")[1])
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
            usedYourPotentialBefore = generalBooleans.Contains("usedYourPotentialBefore");
            unlockedRCT = generalBooleans.Contains("unlockedRCT");
            challengersEye = generalBooleans.Contains("hollowEyes");
            uniqueBodyStructure = generalBooleans.Contains("uniqueBodyStructure");
            blessedByBlackFlash = generalBooleans.Contains("blessedByBlackFlash");
            explosiveCursedEnergy = generalBooleans.Contains("explosiveCursedEnergy");
            sharpCursedEnergy = generalBooleans.Contains("sharpCursedEnergy");
            overflowingEnergy = generalBooleans.Contains("overflowingEnergy");
            sukunasSkull = generalBooleans.Contains("sukunasSkull");

            maxCursedEnergy = calculateBaseMaxCE();
            cursedEnergyRegenPerSecond = calculateBaseCERegenRate();

            if (innateTechnique != null)
            {
                var indexes = tag.GetList<int>("deathPaintings");

                for (int i = 0; i < indexes.Count; i++)
                {
                    deathPaintings[indexes[i]] = true;
                }
            }

            maxBloodEnergy = calculateBaseMaxBE();
             bloodEnergyRegenPerSecond = calculateBaseBERegenRate();


            if (innateTechnique != null)
            {
                var indexes = tag.GetList<int>("sukunasFingers");

                for (int i = 0; i < indexes.Count; i++)
                {
                    sukunasFingers[indexes[i]] = true;
                }
            }

            sixEyesLevel = tag.ContainsKey("sixEyesLevel") ? tag.GetShort("sixEyesLevel") : (short)0;

            var currentQuestsData = tag.GetList<string>("currentQuests");
            foreach (var quest in currentQuestsData)
            {
                currentQuests.Add(Quest.QuestBuilder(quest));
            }

            completedQuests = tag.GetList<string>("completedQuests").ToList();

            mechanicalBossesDefeatedFlags = (byte)(tag.ContainsKey("mechanicalBossesDefeatedFlags") ? tag.GetInt("mechanicalBossesDefeatedFlags") : 0b0000); 
        }
        
        public float calculateBaseMaxCE()
        {
            float baseCost = 100f;
            float sum = 0f;

            if (heavenlyRestriction)
            {
                return baseCost + (35 * numberBossesDefeated);
            }

            if (cursedSkull)
                sum += 100f; // 200 total

            if (cursedMechanicalSoul)
                sum += 300f; // 500 total

            if (cursedPhantasmalEye)
                sum += 500f; // 1000 total

            if (cursedProfaneShards)
                sum += 1000f; // 2000 total

            return baseCost + sum;
        }

        public float calculateBaseMaxBE()
        {
            float baseCost = 100f;
            float[] maxBEBonuses = { 0f, 100f, 200f, 300f, 0f, 400f, 500f, 0f };
            return baseCost + maxBEBonuses.Where((bonus, i) => deathPaintings[i]).Sum();
        }

        public float calculateBaseBERegenRate()
        {
            float baseRegen = 1f;
            float[] regenBonuses = { 5f, 0f, 0f, 0f, 10f, 0f, 0f, 20f };
            return baseRegen + regenBonuses.Where((bonus, i) => deathPaintings[i]).Sum();
        }

        public float calculateBaseCERegenRate()
        {
            float baseRegen = 1f;
            float sum = 0f;

            if (heavenlyRestriction)
            {
                return baseRegen + (numberBossesDefeated * 2.5f);
            }

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
