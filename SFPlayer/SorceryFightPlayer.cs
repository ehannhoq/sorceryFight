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
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.DomainExpansions;
using System.Linq;
using Microsoft.Xna.Framework.Design;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.Content.DomainExpansions.NPCDomains;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        #region Global Variables
        public static readonly float DefaultBurntTechniqueDuration = 30;
        public static readonly float DefaultBrainDamageDuration = 120;

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
        public bool recievedYourPotential;
        public bool yourPotentialSwitch;
        public bool usedYourPotentialBefore;
        public bool usedCursedFists;
        private HashSet<int> npcsHitWithCursedFists;
        public int idleDeathGambleBuffStrength;
        public SorceryFightUI sfUI;
        #endregion

        #region Domain
        public bool inDomainAnimation;
        public int domainTimer;
        public bool HasActiveDomain => DomainExpansionController.ActiveDomains.Any(d => d is PlayerDomainExpansion && d is not ISimpleDomain && d.owner == Player.whoAmI);
        public bool fallingBlossomEmotion;
        public bool inSimpleDomain;
        public bool immuneToDomains => fallingBlossomEmotion || hollowWickerBasket || inSimpleDomain;
        #endregion

        #region Player Attributes
        public bool sixEyes;
        public bool uniqueBodyStructure;
        #endregion

        #region Shrine/Vessel Specific Variables
        public bool[] sukunasFingers;
        public int sukunasFingerConsumed => sukunasFingers.Where(x => x).Count();
        #endregion

        #region RCT
        public bool unlockedRCT;
        public int rctAuraIndex;
        #endregion

        #region Black Flash
        public int blackFlashTime;
        public int blackFlashTimeLeft;
        public int blackFlashCounter;
        public int lowerWindowTime;
        public int upperWindowTime;
        #endregion

        #region UI Positions
        public Vector2 CTSelectorPos;
        public Vector2 PTSelectorPos;
        public Vector2 CEBarPos;
        #endregion
        
        public override void UpdateEquips()
        {
            if (innateTechnique == null) return;
            innateTechnique.UpdateEquips(this);
        }


        public override void UpdateLifeRegen()
        {
            if (innateTechnique == null) return;
            innateTechnique.UpdateLifeRegen(this);
        }


        public override void PreUpdate()
        {
            if (innateTechnique == null || Main.dedServ) return;

            if (preventDeath && deathPosition != Vector2.Zero && Player.position != deathPosition)
            {
                Player.position = deathPosition;
                preventDeath = false;
            }

            innateTechnique.PreUpdate(this);
            RCTAnimation();
            PlayerAttributeIcons();
            Keybinds();

            cursedEnergyRegenPerSecond = 0f;
            maxCursedEnergy = 0f;

            cursedEnergyRegenPerSecond = calculateBaseCERegenRate();
            maxCursedEnergy = calculateBaseMaxCE();


            if (blackFlashTimeLeft != 0)
            {
                if (blackFlashTimeLeft > 0)
                    blackFlashTimeLeft--;
                else if (blackFlashTimeLeft < 0)
                    blackFlashTimeLeft++;

                if (blackFlashTimeLeft == 1)
                {
                    ResetBlackFlashState();
                }
            }
        }

        private int TEMP_disabledRegenTimer = 0;
        public override void PostUpdate()
        {
            cursedEnergyRegenPerSecond += cursedEnergyRegenFromOtherSources;
            maxCursedEnergy += maxCursedEnergyFromOtherSources;

            if (cursedEnergy > 0)
            {
                cursedEnergy -= SFUtils.RateSecondsToTicks(cursedEnergyUsagePerSecond);
            }

            bool disabledRegen = disableRegenFromBuffs || disableRegenFromProjectiles || disableRegenFromDE;

            if (cursedEnergy < maxCursedEnergy && !disabledRegen)
            {
                cursedEnergy += SFUtils.RateSecondsToTicks(cursedEnergyRegenPerSecond);
            }

            if (cursedEnergy > maxCursedEnergy)
            {
                cursedEnergy = maxCursedEnergy;
            }

            if (cursedEnergy < 0)
            {
                cursedEnergy = 0;
            }


            cursedEnergyUsagePerSecond = 0f;
            cursedEnergyRegenFromOtherSources = 0f;
            maxCursedEnergyFromOtherSources = 0f;
            disableRegenFromBuffs = false;
            disableCurseTechniques = false;
            blackFlashTime = 30;

            if (disabledRegen)
            {
                TEMP_disabledRegenTimer++;
                if (TEMP_disabledRegenTimer >= 300)
                {
                    disableRegenFromBuffs = false;
                    disableRegenFromProjectiles = false;
                    disableRegenFromDE = false;
                    TEMP_disabledRegenTimer = 0;
                }
            }
        }


        public override void UpdateDead()
        {
            ResetBuffs();
            deathPosition = Player.position;

            if (rctAnimation)
            {
                PreventDeath();
            }

            if (!rctAnimation && sukunasFingerConsumed >= 1)
            {

                if (Player.HasBuff(ModContent.BuffType<KingOfCursesBuff>()) && innateTechnique.Name == "Shrine")
                    Player.AddBuff(ModContent.BuffType<KingOfCursesBuff>(), SFUtils.BuffSecondsToTicks(15 + (sukunasFingerConsumed * 2.25f)));

                else if (innateTechnique.Name == "Vessel")
                {
                    int chance = SorceryFight.DevModeNames.Contains(Player.name) ? 100 : 5 + (int)(sukunasFingerConsumed * 0.5);
                    if (SFUtils.Roll(chance))
                    {
                        PreventDeath();
                        Player.AddBuff(ModContent.BuffType<KingOfCursesBuff>(), SFUtils.BuffSecondsToTicks(15 + (sukunasFingerConsumed * 2.25f)));
                    }
                }
            }

            disableRegenFromDE = false;
            disableRegenFromProjectiles = false;
        }


        public override void OnEnterWorld()
        {
            if (Main.myPlayer == Player.whoAmI && !recievedYourPotential)
            {
                Player.QuickSpawnItem(Player.GetSource_Misc("YourPotential"), ModContent.ItemType<YourPotential>());
                recievedYourPotential = true;
            }
        }


        void Keybinds()
        {
            if (Player.dead) return;


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


            if (SFKeybinds.DomainExpansion.Current)
            {
                domainTimer++;
                if (HasActiveDomain)
                    domainTimer = 1;

                if (innateTechnique.DomainExpansion.Unlocked(this))
                {
                    float zoom = 1.3f * (MathF.Log10(domainTimer + 1f) + 0.22f);
                    Vector2 zoomVec = new Vector2(zoom, zoom);
                    zoomVec = Vector2.Clamp(zoomVec, Main.BackgroundViewMatrix.Zoom, Vector2.One * 2);

                    CameraController.SetCameraZoom(zoomVec);
                }
            }
            else if (domainTimer != 0)
            {
                if (domainTimer < 20)
                {
                    if (HasActiveDomain)
                        CloseDomainExpansion();
                    else
                    {
                        ToggleSimpleDomain();
                    }
                }
                else if (innateTechnique.DomainExpansion.Unlocked(this))
                {
                    if (DomainExpansionController.ActiveDomains.TryGet(d => d is ISimpleDomain && d.owner == Player.whoAmI, out DomainExpansion de))
                    {
                        DomainExpansionController.CloseDomain(de.id);
                    }

                    ExpandDomainExpansion();
                }

                domainTimer = 0;
                CameraController.ResetCameraZoom();
            }



            if (SFKeybinds.AttemptBlackFlash.JustPressed && blackFlashTimeLeft == 0)
            {
                blackFlashTimeLeft = blackFlashTime;

                int variation = pictureLocket ? Main.rand.Next(-3, 2) : Main.rand.Next(-5, 4);

                lowerWindowTime = innateTechnique.Name == "Vessel" ? 14 - blackFlashCounter / 2 + variation : 15 - blackFlashCounter / 2 + variation;
                upperWindowTime = innateTechnique.Name == "Vessel" ? 16 + blackFlashCounter / 2 + variation : 16 + blackFlashCounter / 2 + variation;
                sfUI.BlackFlashWindow(lowerWindowTime, upperWindowTime);
            }

            // if (SFKeybinds.CursedFist.JustPressed)
            // {
            //     if (Player.HasBuff<BurntTechnique>())
            //     {
            //         int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Your technique is exhausted!");
            //         Main.combatText[index].lifeTime = 60;
            //         return;
            //     }

            //     CursedFist();
            // }

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


        void ExpandDomainExpansion()
        {
            if (Player.whoAmI == Main.myPlayer)
            {
                if (Player.HasBuff<BrainDamage>())
                {
                    int index = CombatText.NewText(Player.getRect(), Color.DarkRed, "You can't use this right now!");
                    Main.combatText[index].lifeTime = 60;
                    return;
                }

                CameraController.ResetCameraZoom();

                foreach (NPC npc in Main.ActiveNPCs)
                {
                    if (npc.GetDomain() != null)
                    {
                        NPCDomainController.playerCastedDomain = true;

                        if (Main.netMode == NetmodeID.MultiplayerClient)
                        {
                            ModPacket packet = ModContent.GetInstance<SorceryFight>().GetPacket();
                            packet.Write((byte)MessageType.PlayerCastingDomain);
                            packet.Write(Player.whoAmI);
                            packet.Send();
                        }
                    }
                }

                if (!inDomainAnimation)
                {
                    inDomainAnimation = true;

                    int index = CombatText.NewText(Player.getRect(), Color.White, "Domain Expansion:");
                    Main.combatText[index].lifeTime = 90;

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        int index = CombatText.NewText(Player.getRect(), Color.White, innateTechnique.DomainExpansion.DisplayName);
                        Main.combatText[index].lifeTime = 90;
                    }, 100);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        DomainExpansionController.ExpandDomain(Player.whoAmI, innateTechnique.DomainExpansion);
                        inDomainAnimation = false;
                    }, 200);
                }
            }
        }


        void CloseDomainExpansion()
        {
            if (DomainExpansionController.ActiveDomains.TryGet(d => d is PlayerDomainExpansion && d.owner == Player.whoAmI, out DomainExpansion de))
            {
                DomainExpansionController.CloseDomain(de.id);
            }
        }

        void ToggleSimpleDomain()
        {
            if (DomainExpansionController.ActiveDomains.TryGet(d => d is ISimpleDomain && d.owner == Player.whoAmI, out DomainExpansion de))
            {
                DomainExpansionController.CloseDomain(de.id);
                return;
            }

            if (Main.myPlayer == Player.whoAmI)
            {
                int index;
                if (!new SimpleDomainFloating().Unlocked(this))
                {
                    index = CombatText.NewText(Player.getRect(), Color.DarkRed, "You haven't unlocked this yet!");
                    Main.combatText[index].lifeTime = 60;
                    return;
                }


                if (Player.HasBuff<BurntTechnique>())
                {
                    index = CombatText.NewText(Player.getRect(), Color.DarkRed, "Your technique is exhausted!");
                    Main.combatText[index].lifeTime = 180;
                    return;
                }

                DomainExpansionController.ExpandDomain(Player.whoAmI, new SimpleDomainFloating());
                index = CombatText.NewText(Player.getRect(), Color.LightCyan, "New Shadow Style: Simple Domain");
                Main.combatText[index].lifeTime = 60;

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
            if (SFUtils.Roll(SFConstants.SixEyesPercent) && !sixEyes)
            {
                sixEyes = true;
                ChatHelper.SendChatMessageToClient(SFUtils.GetNetworkText($"Mods.sorceryFight.Misc.InnateTechniqueUnlocker.PlayerAttributes.SixEyes"), Color.Khaki, Player.whoAmI);
                successfulRoll = true;
            }

            if (SFUtils.Roll(SFConstants.UniqueBodyStructurePercent) && !uniqueBodyStructure)
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

            if (innateTechnique.Name == "Vessel")
                Player.AddBuff(ModContent.BuffType<SukunasVesselBuff>(), 2);

            if (blackFlashCounter > 0)
                Player.AddBuff(ModContent.BuffType<FlowState>(), 2);
        }
    }
}