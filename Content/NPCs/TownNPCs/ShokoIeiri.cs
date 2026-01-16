
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.Localization;
using Terraria.GameContent;
using Terraria.GameContent.Personalities;
using System.Collections.Generic;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.CursedTechniques.Limitless;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.SFPlayer;
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Dialog;
using CalamityMod;
using Microsoft.Xna.Framework;

namespace sorceryFight.Content.NPCs.TownNPCs
{
    [AutoloadHead]
    public class ShokoIeiri : SorceryFightNPC
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
            SFNPC.name = "ShokoIeiri";
            SFNPC.attackDamage = 5;
            SFNPC.knockback = 4f;
            SFNPC.attackCooldown = 30;
            SFNPC.attackProjectile = 0;

            NPC.defense = 15;
            NPC.lifeMax = 200;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;

            base.SetDefaults();
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