using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using Terraria.Audio;

namespace sorceryFight
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        #region Global Variables
        public bool hasUIOpen;
        public bool disableRegenFromProjectiles;
        public bool disableRegenFromBuffs;
        public bool disableRegenFromDE;
        #endregion

        #region Global Cursed Technique Stuff
        public InnateTechnique innateTechnique;
        public CursedTechnique selectedTechnique;
        public float mastery;
        public float cursedEnergy;
        public float maxCursedEnergy;
        public float cursedEnergyRegenPerSecond;
        public float cursedEnergyUsagePerSecond;

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

        #region One-off Variables
        public bool yourPotentialSwitch;
        #endregion

        #region Domain Expansion Variables
        public bool UnlockedDomain
        {
            get
            {
                return CalamityMod.DownedBossSystem.downedDoG;
            }
        }
        public bool expandedDomain;
        public int domainIndex;
        #endregion

        public bool unlockedRCT;
        public int rctAuraIndex;

        public override void Initialize()
        {
            hasUIOpen = false;
            disableRegenFromProjectiles = false;
            disableRegenFromBuffs = false;
            disableRegenFromDE = false;

            innateTechnique = new InnateTechnique();
            selectedTechnique = new CursedTechnique();
            mastery = 0f;
            cursedEnergy = 0f;
            maxCursedEnergy = 100f;
            cursedEnergyRegenPerSecond = 1f;
            cursedEnergyUsagePerSecond = 0f;

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

            yourPotentialSwitch = false;

            expandedDomain = false;
            domainIndex = -1;

            unlockedRCT = false;
            rctAuraIndex = -1;

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
            cursedEnergyRegenModifiers.AddWithCondition("cursedMask", cursedMask);
            cursedEnergyRegenModifiers.AddWithCondition("cursedEffulgentFeather", cursedEffulgentFeather);
            cursedEnergyRegenModifiers.AddWithCondition("cursedRuneOfKos", cursedRuneOfKos);
            tag["cursedEnergyRegenModifiers"] = cursedEnergyRegenModifiers;
            
            var generalBooleans = new List<string>();
            generalBooleans.AddWithCondition("unlockedRCT", unlockedRCT);
            tag["generalBooleans"] = generalBooleans;

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

            var generalBooleans = tag.GetList<string>("generalBooleans");

            unlockedRCT = generalBooleans.Contains("unlockedRCT");

            maxCursedEnergy = calculateMaxCE();
            cursedEnergyRegenPerSecond = calculateCERegenRate();
        }

        public override void PostUpdate()
        {
            innateTechnique.PostUpdate(this);


            if (SFKeybinds.UseTechnique.JustPressed)
                ShootTechnique();


            if (SFKeybinds.UseRCT.Current)
                UseRCT();

            if (SFKeybinds.UseRCT.JustReleased)
                if (rctAuraIndex != -1)
                {
                    Main.projectile[rctAuraIndex].Kill();
                    rctAuraIndex = -1;
                }


            if (SFKeybinds.DomainExpansion.JustReleased)
                DomainExpansion();


            bool disabledRegen = disableRegenFromBuffs || disableRegenFromProjectiles || disableRegenFromDE;

            if (cursedEnergy > 0)
            {
                cursedEnergy -= SorceryFight.TicksToSeconds(cursedEnergyUsagePerSecond);
                if (cursedEnergy < 0)
                {
                    cursedEnergy = 0;
                }
            }

            if (cursedEnergy < maxCursedEnergy && !disabledRegen)
            {
                cursedEnergy += SorceryFight.TicksToSeconds(cursedEnergyRegenPerSecond);
                if (cursedEnergy > maxCursedEnergy)
                {
                    cursedEnergy = maxCursedEnergy;
                }
            }

            cursedEnergyUsagePerSecond = 0f;
        }

        public override void PostUpdateBuffs()
        {
            if (cursedEnergy < 1)
            {
                cursedEnergy = 1;
                Player.AddBuff(ModContent.BuffType<BurntTechnique>(), SorceryFight.SecondsToTicks(10));
            }


            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                if (cursedEnergy <= 1 || Player.HasBuff<BurntTechnique>())
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

        public float calculateCERegenRate()
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

        public void ShootTechnique()
        {
            if (selectedTechnique.Name == "None Selected." || disableRegenFromProjectiles)
            {
                return;
            }

            Vector2 playerPos = Player.MountedCenter;
            Vector2 mousePos = Main.MouseWorld;
            Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * selectedTechnique.Speed;
            var entitySource = Player.GetSource_FromThis();

            selectedTechnique.Shoot(entitySource, playerPos, dir, Player);
        }

        public void UseRCT()
        {
            if (!unlockedRCT)
            {
                int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "You haven't unlocked this yet!");
                Main.combatText[index].lifeTime = 60;
                return;
            }

            if (Player.HasBuff<BurntTechnique>())
            {
                int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Your technique is exhausted!");
                Main.combatText[index].lifeTime = 60;
                return;
            }

            if (Main.myPlayer == Player.whoAmI && rctAuraIndex == -1)
            {
                IEntitySource source = Player.GetSource_FromThis();
                rctAuraIndex = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<ReverseCursedTechniqueAuraProjectile>(), 0, 0, Player.whoAmI);
            }

            if (cursedEnergy >= 10)
            {
                cursedEnergy -= 10;
                Player.Heal(1);
            }
        }
        public void DomainExpansion()
        {
            if (!UnlockedDomain)
            {
                int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "You haven't unlocked this yet!");
                Main.combatText[index].lifeTime = 60;
                return;
            }

            if (Player.HasBuff<BurntTechnique>())
            {
                int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Your technique is exhausted!");
                Main.combatText[index].lifeTime = 60;
                return;
            }

            if (!expandedDomain)
            { 
                innateTechnique.DomainExpansion(this);
                expandedDomain = true;
            }
            else if (domainIndex != -1)
            {
                innateTechnique.CloseDomain(this);
            }
        }
    }
}