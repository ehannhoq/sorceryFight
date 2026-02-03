
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class AtsuyaKusakabe : SorceryFightNPC
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
            SFNPC.name = "AtsuyaKusakabe";
            SFNPC.attackDamage = 10;
            SFNPC.knockback = 4f;
            SFNPC.attackCooldown = 30;
            SFNPC.attackProjectile = 0;

            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;

            base.SetDefaults();
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.downedBoss1)
            {
                return true;
            }
            return false;
        }
    }
}