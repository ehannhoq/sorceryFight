using System;
using System.Collections.Generic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public override int CostPerSecond => 200;
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;
        public static Dictionary<int, float[]> frozenValues;
        public override void SetDefaults()
        {
            DomainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/UnlimitedVoid", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            BackgroundTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/DomainExpansionBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            frozenValues = new Dictionary<int, float[]>();

            Scale = 0.1f;
            BackgroundScale = 0.1f;
            GoalScale = 2f;

            base.SetDefaults();
        }

        public override void AI()
        {
            base.AI();
            
            float logBase = 10f;
            float maxAIValue = 30f;

            if (NPC.ai[0] < 30)
            {
                NPC.Center = Owner.Center;

                float progress = Math.Clamp(NPC.ai[0] / maxAIValue, 0.01f, 1f);
                BackgroundScale = GoalScale * 4 * (float)(Math.Log(progress * (logBase - 1) + 1) / Math.Log(logBase));
            }

            if (NPC.ai[0] > 30 && NPC.ai[0] < 200)
            {
                float progress = Math.Clamp((NPC.ai[0] - 30) / (maxAIValue + 170), 0.01f, 1f);
                Scale = GoalScale * (float)(Math.Log(progress * (logBase - 1) + 1) / Math.Log(logBase));
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

            npc.position = new Vector2(frozenValues[npcID][4], frozenValues[npcID][5]);

            if (!AffectedByFrozenAI(npc))
            {
                return;
            }
            npc.ai[0] = frozenValues[npcID][0];
            npc.ai[1] = frozenValues[npcID][1];
            npc.ai[2] = frozenValues[npcID][2];
            npc.ai[3] = frozenValues[npcID][3];

        }

        public override void Remove(SorceryFightPlayer sfPlayer)
        {
            frozenValues.Clear();
            base.Remove(sfPlayer);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = DomainTexture.Height / FRAME_COUNT;
            int frameY = NPC.frame.Y * frameHeight;

            Vector2 origin = new Vector2(DomainTexture.Width / 2, frameHeight / 2);
            Vector2 bgOrigin = new Vector2(BackgroundTexture.Width / 2, BackgroundTexture.Height / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, DomainTexture.Width, frameHeight);

            spriteBatch.Draw(BackgroundTexture, NPC.Center - Main.screenPosition, default, Color.White, NPC.rotation, bgOrigin, BackgroundScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(DomainTexture, NPC.Center - Main.screenPosition, sourceRectangle, Color.White, NPC.rotation, origin, Scale, SpriteEffects.None, 0f);

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

        private bool AffectedByFrozenAI(NPC npc)
        {
            if (npc.type == NPCID.MoonLordHand)
                return false;
            if (npc.type == NPCID.MoonLordHead)
                return false;

            return true;
        }
    }
}