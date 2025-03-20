using System;
using System.Collections.Generic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;

namespace sorceryFight.Content.DomainExpansions
{
    public class UnlimitedVoid : DomainExpansion
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.Description");
        public override int CostPerSecond { get; set; } =  100;
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;
        public static Dictionary<int, float[]> frozenValues;
        public override void SetDefaults()
        {
            frozenValues = new Dictionary<int, float[]>();

            Scale = 0.1f;
            BackgroundScale = 0.1f;
            GoalScale = 2f;

            base.SetDefaults();
            
            if (Main.dedServ) return;
            DomainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/UnlimitedVoid", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            BackgroundTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/DomainExpansionBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        {
            base.AI();
        
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.friendly)
                {
                    float distance = Vector2.DistanceSquared(proj.Center, NPC.Center);
                    if (distance < SureHitDistance.Squared())
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