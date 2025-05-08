using System;
using System.Collections.Generic;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public class UnlimitedVoid : DomainExpansion
    {
        public static Dictionary<int, float[]> frozenNPCs = new Dictionary<int, float[]>();
        public override string InternalName => "UnlimitedVoid";

        public override SoundStyle CastSound => SorceryFightSounds.UnlimitedVoid;

        public override Texture2D DomainTexture => ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/UnlimitedVoid", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public override float SureHitRange => 1000f;

        public override float Cost => 100f;

        private ref float tick => ref ai[0];
        private ref float whiteFade => ref ai[1];

        public override void SureHitEffect(NPC npc)
        {
            if (!frozenNPCs.ContainsKey(npc.whoAmI))
            {
                frozenNPCs.Add(npc.whoAmI, [npc.position.X, npc.position.Y, npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3]]);
            }

            npc.position = new Vector2(frozenNPCs[npc.whoAmI][0], frozenNPCs[npc.whoAmI][1]);

            if (!AffectedByFrozenAI(npc))
                return;

            npc.ai[0] = frozenNPCs[npc.whoAmI][2];
            npc.ai[1] = frozenNPCs[npc.whoAmI][3];
            npc.ai[2] = frozenNPCs[npc.whoAmI][4];
            npc.ai[3] = frozenNPCs[npc.whoAmI][5];
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.dedServ) return;

            if (tick > 1 && tick < 140)
            {
                DrawInnerDomain(() =>
                {
                    Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                    Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                    Color fadeColor = new Color(255f, 255f, 255f, whiteFade);

                    spriteBatch.Draw(whiteTexture, screenRectangle, fadeColor);
                });
                return;
            }

            if (tick > 140 && tick < 390)
            {
                DrawInnerDomain(() =>
                {
                    Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                    Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

                    spriteBatch.Draw(whiteTexture, screenRectangle, Color.Black);
                });

                if (tick > 340 && tick < 390)
                {
                    DrawInnerDomain(() =>
                    {
                        Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                        Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
                        Color fadeColor = new Color(255f, 255f, 255f, whiteFade);

                        spriteBatch.Draw(whiteTexture, screenRectangle, fadeColor);
                    });
                }
                return;
            }

            Rectangle src = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
            spriteBatch.Draw(DomainTexture, center - Main.screenPosition, src, Color.White, 0f, src.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
        }

        public override void Update()
        {
            base.Update();

            tick++;

            if ((whiteFade += 0.03f) > 1)
                whiteFade = 1;

            if (tick == 340)
                whiteFade = 0;

            if (tick > 140 && tick < 390)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 offsetPos = center + new Vector2(Main.rand.NextFloat(-2000f, 2000f), Main.rand.NextFloat(-2000f, 2000f));
                    Vector2 velocity = center.DirectionTo(offsetPos) * 80f;

                    List<Color> colors = [
                        new Color(91, 91, 245), // blue
                        new Color(201, 110, 235), // magenta
                        new Color(79, 121, 219), // cyan
                        new Color(124, 42, 232), // purple
                    ];

                    int roll = Main.rand.Next(colors.Count);
                    Color color = colors[roll];

                    LineParticle particle = new LineParticle(center, velocity, false, 180, 3, color);
                    GeneralParticleHandler.SpawnParticle(particle);
                }
            }

            if (Main.myPlayer == owner && tick > 1 && tick < 390)
            {
                Player player = Main.player[owner];
                player.Center = center;
            }
        }

        public override void CloseDomain(SorceryFightPlayer sf, bool supressSyncPacket = false)
        {
            frozenNPCs.Clear();
            base.CloseDomain(sf, supressSyncPacket);
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
