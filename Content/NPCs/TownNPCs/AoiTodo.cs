
using sorceryFight.Content.Quests;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class AoiTodo : SorceryFightNPC
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

            SFNPC.name = "AoiTodo";
            SFNPC.attackDamage = 50;
            SFNPC.knockback = 5f;
            SFNPC.attackCooldown = 30;
            SFNPC.attackProjectile = 0;

            NPC.defense = 30;
            NPC.lifeMax = 350;
            NPC.knockBackResist = 0.75f;
            AnimationType = NPCID.Guide;

            AddQuest(new BlackFlashQuestI());
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            return Main.hardMode;
        }
    }
}