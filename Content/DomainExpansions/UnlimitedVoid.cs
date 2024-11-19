using System;
using System.Collections.Generic;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public class UnlimitedVoid : ModNPC
    {
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;

        public Player player;
        private Texture2D texture;
        private Texture2D bgTexture;
        private float goalScale;
        private float scale;
        private float bgScale;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = FRAME_COUNT;
        }
        public override void SetDefaults()
        {
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/UnlimitedVoid", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            bgTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/DomainExpansionBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;


            NPC.damage = 0;
            NPC.width = 1;
            NPC.height = NPC.width;
            NPC.lifeMax = 1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.scale = 0.1f;
            goalScale = 2f;
            scale = 0.1f;
            bgScale = 0.1f;
            NPC.hide = false;
            NPC.behindTiles = true;
        }

        public override void AI()
        {
            player = Main.player[(int)NPC.ai[1]];
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            NPC.ai[0]++;
  
            float logBase = 10f;
            float maxAIValue = 30f;

            if (NPC.ai[0] < 30)
            {
                NPC.Center = player.Center;

                float progress = Math.Clamp(NPC.ai[0] / maxAIValue, 0.01f, 1f);
                bgScale = goalScale * 4 * (float)(Math.Log(progress * (logBase - 1) + 1) / Math.Log(logBase));
            }

            if (NPC.ai[0] > 30 && NPC.ai[0] < 200)
            {
                float progress = Math.Clamp((NPC.ai[0] - 30) / (maxAIValue + 170), 0.01f, 1f);
                scale = goalScale * (float)(Math.Log(progress * (logBase - 1) + 1) / Math.Log(logBase));
            }

            if (NPC.ai[0] < 200)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offsetPos = NPC.Center + new Vector2(Main.rand.NextFloat(-2000f, 2000f), Main.rand.NextFloat(-2000f, 2000f));
                    Vector2 velocity = NPC.Center.DirectionTo(offsetPos) * 40f;

                    LineParticle particle = new LineParticle(NPC.Center, velocity, false, 180, 1, Color.LightSteelBlue);
                    GeneralParticleHandler.SpawnParticle(particle);
                }
            }

            sfPlayer.disableRegenFromDE = true;
            sfPlayer.cursedEnergy -= 1;

            if (sfPlayer.cursedEnergy < 2)
            {
                sfPlayer.disableRegenFromDE = false;
                player.AddBuff(ModContent.BuffType<BurntTechnique>(), SorceryFight.SecondsToTicks(30));

                player = null;
                NPC.active = false;
            }

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.type != NPCID.TargetDummy && npc.type != ModContent.NPCType<SuperDummyNPC>() && !SorceryFight.IsDomain(npc))
                {
                    float distance = Vector2.Distance(npc.Center, NPC.Center);
                    if (distance < 1000f)
                    {
                        int duration = 10;
                        if (npc.IsABoss()) // Using Calamity's method as it has some extra logic that would probably be needed.
                            duration = 5;

                        npc.AddBuff(ModContent.BuffType<UnlimitedVoidBuff>(), SorceryFight.SecondsToTicks(duration));
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = NPC.frame.Y * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Vector2 bgOrigin = new Vector2(bgTexture.Width / 2, bgTexture.Height / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            spriteBatch.Draw(bgTexture, NPC.Center - Main.screenPosition, default, Color.White, NPC.rotation, bgOrigin, bgScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, sourceRectangle, Color.White, NPC.rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}