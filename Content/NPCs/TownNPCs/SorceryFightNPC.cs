using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Dialog;
using Terraria;
using Terraria.GameInput;
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

        public int attackDamage;
        public int attackCooldown;
        public float knockback;

        public int attackProjectile;
        public int attackProjectileDelay = 10;
        public float attackProjectileSpeed = 10f;

        private const float maxInteractionDistance = 300f;

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
        }

        public override bool CanChat()
        {
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return [
                name
            ];
        }

        public override void AI()
        {
            HandleDialog();
        }

        private void HandleDialog()
        {
            if (Main.dedServ) return;

            if (Vector2.Distance(NPC.Center, Main.LocalPlayer.Center) > maxInteractionDistance) return;

            if (Main.mouseRight && Main.mouseRightRelease)
            {
                Rectangle hitbox = NPC.Hitbox;
                Point mouse = Main.MouseWorld.ToPoint();

                if (hitbox.Contains(mouse))
                {
                    if (Main.LocalPlayer.talkNPC == -1)
                    {
                        ModContent.GetInstance<SorceryFightUISystem>().ActivateDialogUI(Dialog.Create(name + ".Interact"), this);
                    }
                }
            }
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
    }
}