
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.Quests;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace sorceryFight.Content.NPCs.TownNPCs
{
    public class GojoSatoru : SorceryFightNPC
    {
        public bool blindfolded = true;
        public override string Texture => blindfolded ? "sorceryFight/Content/NPCs/TownNPCs/GojoSatoru" : "sorceryFight/Content/NPCs/TownNPCs/GojoSatoru_NoBlindfold";
        public override string HeadTexture => blindfolded ? "GojoSatoru_Head" : "GojoSatoru_NoBlindfild_Head";

        private Dictionary<int, Vector2> velocityData = new Dictionary<int, Vector2>();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 26;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 0;
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();

            SFNPC.name = "GojoSatoru";
            SFNPC.attackDamage = 1000;
            SFNPC.knockback = 4f;
            SFNPC.attackCooldown = 10;
            SFNPC.attackProjectileDelay = 10;
            SFNPC.attackProjectile = ModContent.ProjectileType<AmplificationBlue>();
            SFNPC.attackProjectileSpeed = 20f;

            NPC.defense = 100;
            NPC.lifeMax = 10000;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamageFromHostiles = true;

            AnimationType = NPCID.Guide;

            AddQuest(new SixEyesQuestI());
            AddQuest(new SixEyesQuestII());
        }

        public override void AI()
        {
            base.AI();

            float distFromNearestPlayer = float.MaxValue;
            float minDistForInfinity = 300f;
            foreach (Player p in Main.ActivePlayers)
            {
                float distance = Vector2.Distance(NPC.Center, p.Center);
                if (distance < distFromNearestPlayer)
                    distFromNearestPlayer = distance;
            }

            if (distFromNearestPlayer < minDistForInfinity)
                Infinity();
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.downedGolemBoss)
            {
                return true;
            }
            return false;
        }

        private void Infinity()
        {
            float infinityDistance = 50f;

            foreach (Projectile proj in Main.ActiveProjectiles)
            {

                if (proj.hostile)
                {
                    float distance = Vector2.Distance(proj.Center, NPC.Center);
                    if (distance <= infinityDistance)
                    {
                        proj.velocity *= 0.5f;
                        Vector2 vector = NPC.Center.DirectionTo(proj.Center);
                        proj.velocity = vector * (3f + NPC.velocity.Length()) * ((infinityDistance - distance) / 75);
                    }
                }
            }

            foreach (NPC npc in Main.npc)
            {
                if (!npc.friendly && npc.type != NPCID.TargetDummy && npc.active)
                {
                    float distance = Vector2.Distance(npc.Center, NPC.Center);
                    if (distance <= infinityDistance)
                    {
                        if (!velocityData.ContainsKey(npc.whoAmI))
                        {
                            velocityData[npc.whoAmI] = npc.velocity;
                        }

                        npc.velocity *= 0.5f;
                        Vector2 vector = NPC.Center.DirectionTo(npc.Center);
                        npc.velocity = vector * (3f + NPC.velocity.Length()) * ((infinityDistance - distance) / 50);
                    }
                    else if (velocityData.ContainsKey(npc.whoAmI))
                    {
                        npc.velocity = velocityData[npc.whoAmI];
                        velocityData.Remove(npc.whoAmI);
                    }
                }
            }
        }
    }
}