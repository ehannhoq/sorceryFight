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
using Terraria.ID;
using CalamityMod;
using CalamityMod.CalPlayer.Dashes;
using System;
using Terraria.UI;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        #region Global Variables
        public static readonly float DefaultBurntTechniqueDuration = 30;
        public static readonly float DefaultBurntTechniqueDurationFromDE = 120;

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
        public bool usedCursedFists;
        private HashSet<int> npcsHitWithCursedFists;
        public int idleDeathGambleBuffStrength;
        public SorceryFightUI sfUI;
        #endregion

        #region Domain Expansion Variables
        public bool UnlockedDomain
        {
            get
            {
                return bossesDefeated.Contains(ModContent.NPCType<DevourerofGodsHead>());
            }
        }
        public bool expandedDomain;
        public int domainIndex;
        #endregion

        #region Player Attributes
        public bool sixEyes;
        public bool uniqueBodyStructure;
        #endregion

        #region Shrine/Vessel Specific Variables
        public int sukunasFingerConsumed;
        #endregion

        #region RCT
        public bool unlockedRCT;
        public int rctAuraIndex;
        #endregion

        #region Black Flash
        public int blackFlashTime;
        public int blackFlashTimeLeft;
        public bool blackFlashHit; // for UI only
        public int blackFlashCounter;

        #endregion

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

            if (blackFlashTimeLeft > 0)
            {
                if (blackFlashTimeLeft-- < 0)
                    blackFlashTimeLeft = 0;
            }

            disableRegenFromBuffs = false;
            disableCurseTechniques = false;
            blackFlashTime = 120;

            PostAccessoryUpdate();
        }

        public override void UpdateDead()
        {
            ResetBuffs();

            if (rctAnimation)
            {
                PreventRCTAnimDeath();
            }

            disableRegenFromDE = false;
            disableRegenFromProjectiles = false;
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

            if (SFKeybinds.AttemptBlackFlash.JustPressed)
            {
                if (Player.HasBuff<BurntTechnique>())
                {
                    int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Your technique is exhausted!");
                    Main.combatText[index].lifeTime = 60;
                    return;
                }

                blackFlashTimeLeft = blackFlashTime;
            }

            if (SFKeybinds.CursedFist.JustPressed)
            {
                if (Player.HasBuff<BurntTechnique>())
                {
                    int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Your technique is exhausted!");
                    Main.combatText[index].lifeTime = 60;
                    return;
                }

                CursedFist();
            }

            if (usedCursedFists)
            {
                if (Player.dashDelay <= 15)
                    usedCursedFists = false;
                else
                    CalculateCursedFistsHitbox();
            }
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

        void CursedFist()
        {
            if (Player.dashDelay > 0) return;
            npcsHitWithCursedFists.Clear();
            usedCursedFists = true;

            Player.dashDelay = 45;
            float runSpeed = Math.Max(Player.accRunSpeed, Player.maxRunSpeed);
            Player.velocity.X += runSpeed * Player.direction;

            CalculateCursedFistsHitbox();
        }

        void CalculateCursedFistsHitbox()
        {
            Rectangle hitArea = new Rectangle((int)(Player.position.X + Player.velocity.X * 0.5 - 4f), (int)(Player.position.Y + Player.velocity.Y * 0.5 - 4f), Player.width + 8, Player.height + 8);
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npcsHitWithCursedFists.Contains(npc.whoAmI)) continue;
                if (Player.dontHurtCritters && NPCID.Sets.CountsAsCritter[npc.type]) continue;
                if (npc.dontTakeDamage && npc.friendly) continue;

                if (hitArea.Intersects(npc.getRect()) && (npc.noTileCollide || Player.CanHit(npc)))
                {
                    DashHitContext hitContext = new DashHitContext
                    {
                        BaseDamage = 50,
                        BaseKnockback = 6f,
                        HitDirection = Player.direction,
                        damageClass = DamageClass.Melee,
                        PlayerImmunityFrames = 10
                    };

                    int dashDamage = (int)Player.GetTotalDamage(hitContext.damageClass).ApplyTo(hitContext.BaseDamage);
                    float dashKB = Player.GetTotalKnockback(hitContext.damageClass).ApplyTo(hitContext.BaseKnockback);
                    bool rollCrit = Main.rand.Next(100) < Player.GetTotalCritChance(hitContext.damageClass);

                    Player.ApplyDamageToNPC(npc, dashDamage, dashKB, hitContext.HitDirection, rollCrit, hitContext.damageClass, true);
                    Player.GiveImmuneTimeForCollisionAttack(hitContext.PlayerImmunityFrames);

                    npcsHitWithCursedFists.Add(npc.whoAmI);
                }
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

            if (SFUtils.Roll(SFConstants.SukunasVesselDenominator) && innateTechnique.Name == "Shrine")
            {
                innateTechnique = new VesselTechnique();
                ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText($"Mods.sorceryFight.Misc.InnateTechniqueUnlocker.PlayerAttributes.SukunasVessel"), Color.Khaki, Player.whoAmI);
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

            if (innateTechnique.Name == "Vessel")
                Player.AddBuff(ModContent.BuffType<SukunasVesselBuff>(), 2);
        }
    }
}