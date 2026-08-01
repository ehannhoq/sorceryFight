using sorceryFight.Content.Cutscenes;
using sorceryFight.Content.Cutscenes.MahoragaCutscene;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace sorceryFight.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class MegumiFushiguro : SorceryFightNPC
    {
        public bool summoningMahoraga = false;
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

            SFNPC.name = "MegumiFushiguro";
            SFNPC.attackDamage = 20;
            SFNPC.knockback = 10f;
            SFNPC.attackCooldown = 15;
            SFNPC.attackProjectile = 0;

            NPC.defense = 20;
            NPC.lifeMax = 150;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }


        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (NPC.downedMechBoss3)
            {
                return true;
            }
            return false;
        }

        public void SummonMahoragaBoss() {
            if (Main.dedServ) return;
            CutsceneManager.QueueCutscene(new MahoragaCutscene());
        }

        public override void FindFrame(int frameHeight)
        {
            if (summoningMahoraga)
            {
                NPC.frame.Y = frameHeight * 25;
                return;
            }
            base.FindFrame(frameHeight);
        }
    }
}
