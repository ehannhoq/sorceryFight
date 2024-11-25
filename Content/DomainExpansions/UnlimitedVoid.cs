using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Limitless;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public class UnlimitedVoid : DomainExpansion
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.LockedDescription");
        public override Player Owner { get; set; }
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;
        public static Dictionary<int, float[]> frozenValues;
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
            NPC.hide = true;
            NPC.behindTiles = false;
            frozenValues = new Dictionary<int, float[]>();
        }

        public override void AI()
        {
            if (Owner == null)
            {
                Owner = Main.player[(int)NPC.ai[1]];
            }

            Owner = Main.player[(int)NPC.ai[1]];
            SorceryFightPlayer sfPlayer = Owner.GetModPlayer<SorceryFightPlayer>();
            NPC.ai[0]++;

            if (!NPC.active && BossRushEvent.BossRushActive)
            {
                NPC.active = true;
            }

            if (Owner.dead)
            {
                Remove(sfPlayer);
            }
  
            float logBase = 10f;
            float maxAIValue = 30f;

            if (NPC.ai[0] < 30)
            {
                NPC.Center = Owner.Center;

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

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.friendly)
                {
                    float distance = Vector2.Distance(proj.Center, NPC.Center);
                    if (distance < 1000f)
                    {
                        proj.Kill();
                    }
                }
            }
        }

        public override void NPCDomainEffect(NPC npc)
        {
            int npcID = npc.whoAmI;
            if (!frozenValues.ContainsKey(npcID))
            {
                frozenValues[npcID] = new float[6];
                float[] data = [npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3], npc.position.X, npc.position.Y];
                Array.Copy(data, frozenValues[npcID], 6);
            }

            npc.ai[0] = frozenValues[npcID][0];
            npc.ai[1] = frozenValues[npcID][1];
            npc.ai[2] = frozenValues[npcID][2];
            npc.ai[3] = frozenValues[npcID][3];
            npc.position = new Vector2(frozenValues[npcID][4], frozenValues[npcID][5]);
        }

        private void Remove(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.disableRegenFromDE = false;
            sfPlayer.domainIndex = -1;
            sfPlayer.expandedDomain = false;
            Owner.AddBuff(ModContent.BuffType<BurntTechnique>(), SorceryFight.BuffSecondsToTicks(30));
            Owner = null;
            frozenValues.Clear();

            NPC.active = false;
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

        public override void DrawBehind(int index)
        {
            List<int> newCache = new List<int>(200)
            {
                index
            };

            foreach (int i in Main.instance.DrawCacheNPCsMoonMoon)
            {
                newCache.Add(i);
            }

            Main.instance.DrawCacheNPCsMoonMoon = newCache;
        }
    }
}