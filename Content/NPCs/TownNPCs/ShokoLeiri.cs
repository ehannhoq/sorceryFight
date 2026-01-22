
using System;
using sorceryFight.Content.Quests;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace sorceryFight.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class ShokoLeiri : SorceryFightNPC
    {
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

            SFNPC.name = "ShokoLeiri";
            SFNPC.attackDamage = 5;
            SFNPC.knockback = 4f;
            SFNPC.attackCooldown = 30;
            SFNPC.attackProjectile = 0;

            NPC.defense = 15;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;

            AddQuest(new ShokoQuestI());
        }


        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.downedMechBoss3)
            {
                return true;
            }
            return false;
        }
    }
}