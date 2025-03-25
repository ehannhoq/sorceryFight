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
using Terraria.GameContent;

namespace sorceryFight.Content.DomainExpansions
{
    public class UnlimitedVoid : DomainExpansion
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.Description");
        public override int CostPerSecond { get; set; } = 100;
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;
        public static Dictionary<int, float[]> frozenValues;
        public override void SetDefaults()
        {
            frozenValues = new Dictionary<int, float[]>();

            Scale = 0f;
            BackgroundScale = 0f;
            GoalScale = 2f;

            base.SetDefaults();

            if (Main.dedServ) return;
            DomainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/UnlimitedVoid", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        {
            base.AI();

            if (NPC.ai[0] < 200)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offsetPos = NPC.Center + new Vector2(Main.rand.NextFloat(-2000f, 2000f), Main.rand.NextFloat(-2000f, 2000f));
                    Vector2 velocity = NPC.Center.DirectionTo(offsetPos) * 80f;

                    List<Color> colors = [
                        new Color(91, 91, 245), // blue
                        new Color(201, 110, 235), // magenta
                        new Color(79, 121, 219), // cyan
                        new Color(124, 42, 232), // purple
                    ];

                    int roll = Main.rand.Next(colors.Count);
                    Color color = colors[roll];

                    LineParticle particle = new LineParticle(NPC.Center, velocity, false, 180, 3, color);
                    GeneralParticleHandler.SpawnParticle(particle);
                }

                if (NPC.ai[0] > 100)
                {
                    NPC.ai[2] = 1 * (NPC.ai[0] - 100) / 100f;
                }
            }

            if (NPC.ai[0] > 200)
            {
                Scale = GoalScale;
            }

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



        private bool AffectedByFrozenAI(NPC npc)
        {
            if (npc.type == NPCID.MoonLordHand)
                return false;
            if (npc.type == NPCID.MoonLordHead)
                return false;

            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = DomainTexture.Height;
            int frameY = NPC.frame.Y * DomainTexture.Height;

            Vector2 origin = new Vector2(DomainTexture.Width / 2, frameHeight / 2);
            Vector2 bgOrigin = new Vector2(BackgroundTexture.Width / 2, BackgroundTexture.Height / 2);
            Rectangle sourceRectangle = new Rectangle(0, frameY, DomainTexture.Width, frameHeight);

            spriteBatch.Draw(BackgroundTexture, NPC.Center - Main.screenPosition, default, Color.White, NPC.rotation, bgOrigin, BackgroundScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(DomainTexture, NPC.Center - Main.screenPosition, sourceRectangle, Color.White, NPC.rotation, origin, Scale, SpriteEffects.None, 0f);

            if (NPC.ai[0] > 100 && NPC.ai[0] < 200)
                DrawWhiteScreenForPlayers(spriteBatch);

            return false;
        }

        void DrawWhiteScreenForPlayers(SpriteBatch spriteBatch)
        {
            foreach (Player player in Main.player)
            {
                if (player.active && Vector2.DistanceSquared(player.Center, NPC.Center) <= SureHitDistance.Squared())
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        
                        Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                        Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                        Color fadeColor = Color.White * MathHelper.Clamp(NPC.ai[2], 0f, 1f);

                        spriteBatch.Draw(whiteTexture, screenRectangle, fadeColor);
                    }
                }
            }
        }
    }
}