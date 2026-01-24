using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Quests;
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Dialog;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.TownNPCs
{
    public abstract class SorceryFightNPC : ModNPC
    {
        public SorceryFightNPC SFNPC => this;

        /// <summary>
        /// The name of the NPC. Used as the name in game, as well as to retrieve dialog keys.
        /// </summary>
        public string name;
        private List<Quest> quests;
        private List<Quest> completedQuests;

        public int attackDamage;
        public int attackCooldown;
        public float knockback;

        public int attackProjectile;
        public int attackProjectileDelay = 10;
        public float attackProjectileSpeed = 10f;

        private const float maxInteractionDistance = 150f;

        private void HandleDialog()
        {
            if (Main.dedServ) return;

            if (Vector2.Distance(NPC.Center, Main.LocalPlayer.Center) > maxInteractionDistance) return;

            Rectangle hitbox = NPC.Hitbox;
            Point mouse = Main.MouseWorld.ToPoint();

            if (hitbox.Contains(mouse))
            {
                if (Main.mouseRight && Main.mouseRightRelease)
                {
                    if (Main.LocalPlayer.talkNPC == -1)
                    {
                        SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();

                        foreach (Quest npcQuest in quests)
                        {
                            foreach (Quest playerQuest in sfPlayer.currentQuests)
                            {
                                if (playerQuest.completed && playerQuest.GetClass() == npcQuest.GetClass())
                                {
                                    sfPlayer.CompleteQuest(playerQuest);
                                    completedQuests.Add(npcQuest);
                                    ModContent.GetInstance<SorceryFightUISystem>().ActivateDialogUI(Dialog.Create($"{name}.CompletedQuest"), this);
                                    return;
                                }
                            }
                        }
                        
                        ModContent.GetInstance<SorceryFightUISystem>().ActivateDialogUI(Dialog.Create($"{name}.Interact"), this);
                    }
                }
            }
        }

        public void CompletedQuest()
        {
            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();
            foreach (Quest q in completedQuests)
            {
                q.GiveRewards(sfPlayer);
            }
            completedQuests = new List<Quest>();
        }

        public void AddQuest(Quest quest)
        {
            quests.Add(quest);
        }

        public bool GetQuestIfAvailable(SorceryFightPlayer sf, [NotNullWhen(true)] out Quest quest)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                if (sf.completedQuests.Contains(quests[i].GetClass())) continue;

                if (quests[i].IsAvailable(sf))
                {
                    quest = quests[i];
                    return true;
                }
            }
            quest = null;
            return false;
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            quests = new List<Quest>();
            completedQuests = new List<Quest>();
        }

        public override bool CanChat()
        {
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return [
                SplitFullName(name)
            ];
        }

        public override void AI()
        {
            HandleDialog();
        }



        public override bool UsesPartyHat()
        {
            return true;
        }


        public override bool CanGoToStatue(bool toKingStatue)
        {
            return toKingStatue;
        }


        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = attackDamage;
            knockback = this.knockback;
        }


        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = attackCooldown;
            randExtraCooldown = 0;
        }


        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = attackProjectile;
            attackDelay = attackProjectileDelay;
        }


        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = attackProjectileSpeed;
        }

        private static string SplitFullName(string name)
        {
            for (int i = 1; i < name.Length; i++)
            {
                if (char.IsUpper(name[i]))
                {
                    return string.Concat(name.AsSpan(0, i), " ", name.AsSpan(i));
                }
            }

            return name;
        }
    }
}