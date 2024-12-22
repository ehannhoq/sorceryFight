using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public abstract class DomainExpansion : ModNPC
    {
        public override LocalizedText DisplayName { get; }
        public abstract string Description { get; }
        public abstract string LockedDescription { get; }
        public abstract Player Owner { get; set; }
        public abstract int CostPerSecond { get; }
        public abstract void NPCDomainEffect(NPC npc);

        public virtual Texture2D DomainTexture { get; set; }
        public virtual Texture2D BackgroundTexture { get; set; }
        public virtual float Scale { get; set; }
        public virtual float BackgroundScale { get; set; }
        public virtual float GoalScale { get; set; }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.damage = 0;
            NPC.width = 1;
            NPC.height = NPC.width;
            NPC.lifeMax = 1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.scale = 0.1f;
            NPC.hide = true;
            NPC.behindTiles = false;
            NPC.boss = true;
        }
        public override void AI()
        {
            Owner??= Main.player[(int)NPC.ai[1]];
            SorceryFightPlayer sfPlayer = Owner.GetModPlayer<SorceryFightPlayer>();


            if (!NPC.active && BossRushEvent.BossRushActive)
            {
                NPC.active = true;
            }

            sfPlayer.disableRegenFromDE = true;
            sfPlayer.cursedEnergy -= SorceryFight.RateSecondsToTicks(CostPerSecond);

            if (Owner.dead || sfPlayer.cursedEnergy < 2)
            {

                Remove(sfPlayer);
            }

            
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.type != NPCID.TargetDummy && npc.type != ModContent.NPCType<SuperDummyNPC>() && !npc.IsDomain())
                {
                    float distance = Vector2.Distance(npc.Center, NPC.Center);
                    if (distance < 1000f)
                    {
                        NPCDomainEffect(npc);
                    }
                }
            }

            NPC.ai[0]++;
        }

        public virtual void Remove(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.disableRegenFromDE = false;
            sfPlayer.domainIndex = -1;
            sfPlayer.expandedDomain = false;
            Owner.AddBuff(ModContent.BuffType<BurntTechnique>(), SorceryFight.BuffSecondsToTicks(210));
            Owner = null;
            NPC.active = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.ai[0]);
            writer.Write(NPC.ai[1]);
            writer.Write(Scale);
            writer.Write(BackgroundScale);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.ai[0] = reader.ReadSingle();
            NPC.ai[1] = reader.ReadSingle();
            Scale = reader.ReadSingle();
            BackgroundScale = reader.ReadSingle();
        }
    }
}
