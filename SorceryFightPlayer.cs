using System.Collections.Generic;
using System.Security.Permissions;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.PassiveTechniques;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        #region Global Variables
        public bool hasUIOpen;
        public bool disabledRegen;
        #endregion

        #region Global Cursed Technique Stuff
        public InnateTechnique innateTechnique;
        public CursedTechnique selectedTechnique;
        public float mastery;
        public float cursedEnergy;
        public float maxCursedEnergy;
        public float cursedEnergyRegenPerSecond;

        #endregion

        #region Max Cursed Energy Modifiers
        public bool cursedSkull;
        public bool cursedMechanicalSoul;
        public bool cursedPhantasmalEye;
        public bool cursedProfaneShards;
        #endregion

        #region Cursed Energy Regen Modifiers
        public bool cursedEye;
        public bool cursedFlesh;
        public bool cursedBulb;
        public bool cursedMask;
        public bool cursedEffulgentFeather;
        public bool cursedRuneOfKos;
        #endregion

        public override void Initialize()
        {
            hasUIOpen = false;
            disabledRegen = false;

            innateTechnique = new InnateTechnique();
            selectedTechnique = new CursedTechnique();
            mastery = 0f;
            cursedEnergy = 0f;
            maxCursedEnergy = 100f;
            cursedEnergyRegenPerSecond = 1f;

            cursedSkull = false;
            cursedMechanicalSoul = false;
            cursedPhantasmalEye = false;
            cursedProfaneShards = false;

            cursedEye = false;
            cursedFlesh = false;
            cursedBulb = false;
            cursedMask = false;
            cursedEffulgentFeather = false;
            cursedRuneOfKos = false;
        }
        public override void SaveData(TagCompound tag)
        {
            tag["innateTechnique"] = innateTechnique.Name;
            tag["mastery"] = mastery;
            tag["cursedEnergy"] = cursedEnergy;

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
            cursedEnergyRegenModifiers.AddWithCondition("curesdMask", cursedMask);
            cursedEnergyRegenModifiers.AddWithCondition("cursedEffulgentFeather", cursedEffulgentFeather);
            cursedEnergyRegenModifiers.AddWithCondition("cursedRuneOfKos", cursedRuneOfKos);
            tag["cursedEnergyRegenModifiers"] = cursedEnergyRegenModifiers;
        }

        public override void LoadData(TagCompound tag)
        {
            string innateTechniqueName = tag.ContainsKey("innateTechnique") ? tag.GetString("innateTechnique") : "";
            innateTechnique = InnateTechnique.GetInnateTechnique(innateTechniqueName);
            
            mastery = tag.ContainsKey("mastery") ? tag.GetFloat("mastery") : 0f;
            cursedEnergy = tag.ContainsKey("cursedEnergy") ? tag.GetFloat("cursedEnergy") : 0f;

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

            maxCursedEnergy = calculateMaxCE();
            cursedEnergyRegenPerSecond = calculateCERegenRate();
        }

        public override void PostUpdate()
        {
            if (SFKeybinds.UseTechnique.JustPressed)
            {
                ShootTechnique();
            }

            if (cursedEnergy < maxCursedEnergy && !disabledRegen)
            {
                cursedEnergy += SorceryFight.TicksToSeconds(cursedEnergyRegenPerSecond);
                if (cursedEnergy > maxCursedEnergy)
                {
                    cursedEnergy = maxCursedEnergy;
                }
            }
        }

        public override void PostUpdateBuffs()
        {
            if (cursedEnergy < 1)
            {
                cursedEnergy = 1;
                Player.AddBuff(ModContent.BuffType<BurntTechnique>(), (int)SorceryFight.SecondsToTicks(10));
            }

            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                if (cursedEnergy <= 1)
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

        public float calculateMaxCE()
        {
            float baseCE = 500f;
            float sum = 0f;

            if (cursedSkull)
                sum += 100f; // 200 total

            if (cursedMechanicalSoul)
                sum += 300f; // 500 total

            if (cursedPhantasmalEye)
                sum += 500f; // 1000 total

            if (cursedProfaneShards)
                sum += 1000f; // 2000 total

            float total = baseCE + sum;

            if (Main.expertMode)
                total *= 1.5f;

            if (Main.masterMode)
                total *= 2.0f;


            return total;
        }

        public float calculateCERegenRate()
        {
            float baseRegen = 10f;
            float sum = 0f;

            if (cursedEye) 
                sum += 4f; // 5 CE/s

            if (cursedFlesh)
                sum += 10f; // 10 CE/s total

            if (cursedBulb)
                sum += 15f; // 25 CE/s total
            
            if (cursedMask) 
                sum += 25f; // 50 CE/s total

            if (cursedEffulgentFeather)
                sum += 25f; // 75 CE/s

            if (cursedRuneOfKos)
                sum += 25f; // 100 CE/s total

            float total = baseRegen + sum;

            if (Main.expertMode && !Main.masterMode)
                total *= 1.5f;

            if (Main.masterMode)
                total *= 2.0f;


            return total;
        }

        public void ShootTechnique()
        {
            if (selectedTechnique.Name == "None Selected.")
            {
                return;
            }

            Vector2 playerPos = Player.MountedCenter;
            Vector2 mousePos = Main.MouseWorld;
            Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * selectedTechnique.Speed;
            var entitySource = Player.GetSource_FromThis();

            selectedTechnique.Shoot(entitySource, playerPos, dir, Player);
        }
    }
}