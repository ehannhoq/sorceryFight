using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Projectiles.Damageable;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.NPCDomains
{
    public class FieldOfHallowedButterflies : NPCDomainExpansion
    {
        public override string InternalName => "FieldOfHallowedButterflies";

        public override SoundStyle CastSound => SorceryFightSounds.PhantasmicLabyrinth;

        public override int Tier => 2;

        public override float SureHitRange => 1150f;

        public override bool ClosedDomain => true;

        Texture2D prismTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/NPCDomains/FieldOfHallowedButterfliesPrism", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        int prismFrame = 0;
        int prismFrameTime = 0;
        const int prismFrames = 7;
        const int prismTickPerFrame = 6;

        int counter = 0;
        const int ticksPerPrismaticLacewing = 7;
        Dictionary<int, List<int>> lacewingMap = new();
        Dictionary<int, bool> lacewingHit = new();
        Dictionary<int, int> lacewingAge = new();

        Dictionary<int, Vector2> playerVelocities = new();

        Dictionary<int, Vector2> playerPositions = new();

        public override bool ExpandCondition(NPC npc)
        {
            if (npc.life > npc.lifeMax * 0.01f && npc.life <= npc.lifeMax * 0.95f) return true;
            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawInnerDomain(() =>
            {
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, screenRectangle, new Color(0, 0, 0, 255));

                Rectangle domainTextureSrc = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
                spriteBatch.Draw(DomainTexture, center - Main.screenPosition, domainTextureSrc, Color.White, 0f, domainTextureSrc.Size() * 0.5f, 2, SpriteEffects.None, 0f);

                if (prismFrameTime++ >= prismTickPerFrame)
                {
                    prismFrameTime = 0;
                    if (prismFrame++ >= prismFrames - 1)
                        prismFrame = 0;
                }

                int frameHeight = prismTexture.Height / prismFrames;
                int frameY = frameHeight * prismFrame;

                foreach (Vector2 pos in playerPositions.Values)
                {
                    Vector2 toPlayer = pos - center;
                    Vector2 toCenter = Main.npc[owner].Center - center;
                    int beamWidth = 2;

                    Rectangle rainbowBeamSrc = new Rectangle(0, 0, (int)toPlayer.Length(), beamWidth);
                    Rectangle whiteBeamSrc = new Rectangle(0, 0, (int)toCenter.Length(), beamWidth);

                    Vector2 toPlayerPerp = new Vector2(-toPlayer.Y, toPlayer.X).SafeNormalize(Vector2.UnitX);
                    Color[] rainbowColors = [Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet];
                    for (int i = -3; i <= 2; i++)
                    {
                        Color rainbowColor = rainbowColors[i + 3];
                        spriteBatch.Draw(TextureAssets.MagicPixel.Value, center + toPlayer / 2 + toPlayerPerp * i * beamWidth - Main.screenPosition, rainbowBeamSrc, rainbowColor, toPlayer.ToRotation(), rainbowBeamSrc.Size() * 0.5f, 1.0f, SpriteEffects.None, 0f);
                    }

                    spriteBatch.Draw(TextureAssets.MagicPixel.Value, center + toCenter / 2 - Main.screenPosition, whiteBeamSrc, Color.White, toCenter.ToRotation(), whiteBeamSrc.Size() * 0.5f, 1.0f, SpriteEffects.None, 0f);
                }

                Rectangle prismSrc = new Rectangle(0, frameY, prismTexture.Width, frameHeight);
                spriteBatch.Draw(prismTexture, center - Main.screenPosition, prismSrc, Color.White, 0f, prismSrc.Size() * 0.5f, 2, SpriteEffects.None, 0f);
            },

            () => spriteBatch.Draw(BaseTexture, center - Main.screenPosition, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height), Color.Black, 0f, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height).Size() * 0.5f, 2f, SpriteEffects.None, 0f)
            );
        }

        public override void SureHitEffect(Player player)
        {
            playerPositions[player.whoAmI] = player.Center;

            int spawnDistanceOffset = 250;
            if (counter++ % ticksPerPrismaticLacewing == 1)
            {
                Vector2 spawnPos = player.Center + Main.rand.NextVector2CircularEdge(spawnDistanceOffset, spawnDistanceOffset);
                int index = NPC.NewNPC(null, (int)spawnPos.X, (int)spawnPos.Y, NPCID.EmpressButterfly);

                playerVelocities[player.whoAmI] = player.velocity;

                if (!lacewingMap.ContainsKey(player.whoAmI))
                    lacewingMap[player.whoAmI] = new List<int>();

                if (index >= 0 && Main.npc[index].active)
                {
                    NPC lacewing = Main.npc[index];
                    lacewing.netUpdate = true;
                    lacewing.velocity = SFUtils.GetIntersectingVelocity(player.Center, spawnPos, player.velocity, 50f);
                    lacewingMap[player.whoAmI].Add(index);
                }
            }

            if (lacewingMap.TryGetValue(player.whoAmI, out List<int> indicies))
            {
                foreach (int i in indicies)
                {
                    if (Main.npc[i].active)
                    {
                        NPC lacewing = Main.npc[i];
                        lacewing.immortal = true;
                        lacewing.dontTakeDamage = true;

                        if (!lacewingHit.ContainsKey(i))
                            lacewingHit[i] = false;
                        if (!lacewingAge.ContainsKey(i))
                            lacewingAge[i] = 0;

                        if (!lacewingHit[i])
                        {
                            if (lacewing.getRect().Intersects(player.getRect()))
                            {
                                player.Hurt(PlayerDeathReason.ByNPC(owner), 45, 0);
                                lacewingHit[i] = true;
                                lacewing.netUpdate = true;
                            }
                        }

                        if (Vector2.Dot(playerVelocities[player.whoAmI], player.velocity) <= 0.8f)
                        {
                            lacewing.velocity = SFUtils.GetIntersectingVelocity(player.Center, lacewing.Center, player.velocity, 50f);
                            playerVelocities[player.whoAmI] = player.velocity;
                            lacewing.netUpdate = true;
                        }

                        if (lacewingAge[i]++ >= 60)
                        {
                            lacewing.active = false;
                            lacewing.netUpdate = true;
                            lacewingHit.Remove(i);
                            lacewingAge.Remove(i);
                        }
                    }
                }
                indicies.RemoveAll(i => !Main.npc[i].active);
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnClose()
        {
            prismFrame = 0;
            counter = 0;
            playerPositions = new();
            playerVelocities = new();
            lacewingMap = new();
            lacewingHit = new();
            lacewingAge = new();
        }
    }
}
