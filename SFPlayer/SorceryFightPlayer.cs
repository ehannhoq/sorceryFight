using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using CalamityMod.NPCs.DevourerofGods;
using sorceryFight.Content.Buffs.PlayerAttributes;
using Terraria.Chat;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        #region Global Variables
        internal static readonly float defaultBurntTechniqueDuration = 30;
        internal static readonly float defaultBurntTechniqueDurationFromDE = 120;

        public bool hasUIOpen;
        public bool disableRegenFromProjectiles;
        public bool disableRegenFromBuffs;
        public bool disableRegenFromDE;
        public bool disableCurseTechniques;
        #endregion

        #region Global Cursed Technique Stuff
        public InnateTechnique innateTechnique;
        public CursedTechnique selectedTechnique;
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
        public float maxCursedEnergyFromOtherSources;
        #endregion

        #region Cursed Energy Regen Modifiers
        public bool cursedEye;
        public bool cursedFlesh;
        public bool cursedBulb;
        public bool cursedMask;
        public bool cursedEffulgentFeather;
        public bool cursedRuneOfKos;
        public float cursedEnergyRegenFromOtherSources;
        #endregion

        #region One-off Variables
        public bool yourPotentialSwitch;
        public bool usedYourPotentialBefore;
        #endregion

        #region Domain Expansion Variables
        public bool UnlockedDomain
        {
            get
            {
                return HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
            }
        }

        #region Player Attributes
        public bool sixEyes;
        public bool uniqueBodyStructure;
        #endregion
        public bool expandedDomain;
        public int domainIndex;
        #endregion

        public bool unlockedRCT;
        public int rctAuraIndex;

        public override void PreUpdate()
        {
            PreAccessoryUpdate();
        }

        public override void PostUpdate()
        {
            if (innateTechnique == null) return;

            innateTechnique.PostUpdate(this);
            PostAnimUpdate();
            PlayerAttributeIcons();
            Keybinds();

            cursedEnergyRegenPerSecond = 0f;
            maxCursedEnergy = 0f;

            cursedEnergyRegenPerSecond = calculateBaseCERegenRate() + cursedEnergyRegenFromOtherSources;
            maxCursedEnergy = calculateBaseMaxCE() + maxCursedEnergyFromOtherSources;

            if (cursedEnergy > 0)
            {
                cursedEnergy -= SorceryFight.RateSecondsToTicks(cursedEnergyUsagePerSecond);
            }

            bool disabledRegen = disableRegenFromBuffs || disableRegenFromProjectiles || disableRegenFromDE;

            if (cursedEnergy < maxCursedEnergy && !disabledRegen)
            {
                cursedEnergy += SorceryFight.RateSecondsToTicks(cursedEnergyRegenPerSecond);
            }

            cursedEnergyUsagePerSecond = 0f;
            cursedEnergyRegenFromOtherSources = 0f;
            maxCursedEnergyFromOtherSources = 0f;


            if (cursedEnergy > maxCursedEnergy)
            {
                cursedEnergy = maxCursedEnergy;
            }

            if (cursedEnergy < 0)
            {
                cursedEnergy = 0;
            }

            disableRegenFromBuffs = false;
            disableCurseTechniques = false;
        }

        public override void UpdateDead()
        {
            ResetBuffs();

            if (rctAnimation)
            {
                PreventRCTAnimDeath();
            }
        }
        void Keybinds()
        {
            if (SFKeybinds.UseTechnique.JustPressed)
            {
                if (!disableCurseTechniques || uniqueBodyStructure)
                    ShootTechnique();
            }


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
        }
        public void ShootTechnique()
        {
            if (selectedTechnique == null || disableRegenFromProjectiles)
            {
                return;
            }

            if (Player.HasBuff<BurntTechnique>())
            {
                int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Your technique is exhausted!");
                Main.combatText[index].lifeTime = 180;
                return;
            }

            if (cursedEnergy < selectedTechnique.CalculateTrueCost(this))
            {
                int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Not enough Cursed Energy!");
                Main.combatText[index].lifeTime = 180;
                return;
            }

            selectedTechnique.UseTechnique(this);
        }

        void UseRCT()
        {
            if (!unlockedRCT)
            {
                return;
            }

            if (Player.HasBuff<BurntTechnique>())
            {
                return;
            }

            if (Player.statLife >= Player.statLifeMax2)
            {
                return;
            }

            if (Main.myPlayer == Player.whoAmI && rctAuraIndex == -1)
            {
                IEntitySource source = Player.GetSource_FromThis();
                rctAuraIndex = Projectile.NewProjectile(source, Player.Center, Vector2.Zero, ModContent.ProjectileType<ReverseCursedTechniqueAuraProjectile>(), 0, 0, Player.whoAmI);
            }

            if (cursedEnergy >= 5)
            {
                cursedEnergy -= 5;
                Player.Heal(1);
            }
        }
        void DomainExpansion()
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
                innateTechnique.ExpandDomain(this);
                expandedDomain = true;
            }
            else if (domainIndex != -1)
            {
                innateTechnique.CloseDomain(this);
            }
        }

        public void RollForPlayerAttributes(bool isReroll = false)
        {
            bool successfulRoll = false;
            if (SFUtils.Roll(SFConstants.SixEyesDenominator) && !sixEyes)
            {
                sixEyes = true;
                ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText($"Mods.sorceryFight.Misc.InnateTechniqueUnlocker.PlayerAttributes.SixEyes"), Color.Khaki, Player.whoAmI);
                successfulRoll = true;
            }

            if (SFUtils.Roll(SFConstants.UniqueBodyStructureDenominator) && !uniqueBodyStructure)
            {
                uniqueBodyStructure = true;
                ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText($"Mods.sorceryFight.Misc.InnateTechniqueUnlocker.PlayerAttributes.UniqueBodyStructure"), Color.Khaki, Player.whoAmI);
                successfulRoll = true;
            }

            if (isReroll && !successfulRoll)
            {
                ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText($"Mods.sorceryFight.Misc.InnateTechniqueUnlocker.PlayerAttributes.FailedReroll"), Color.Khaki, Player.whoAmI);
            }
        }

        void PlayerAttributeIcons()
        {
            if (sixEyes)
                Player.AddBuff(ModContent.BuffType<SixEyesBuff>(), 2);

            if (uniqueBodyStructure)
                Player.AddBuff(ModContent.BuffType<UniqueBodyStructureBuff>(), 2);
        }
    }
}