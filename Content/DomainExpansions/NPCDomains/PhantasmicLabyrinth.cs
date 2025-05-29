using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.NPCDomains
{
    public class PhantasmicLabyrinth : NPCDomainExpansion
    {
        public override string InternalName => "PhantasmicLabyrinth";

        public override SoundStyle CastSound => SorceryFightSounds.UnlimitedVoid;

        public override float SureHitRange => 1150f;

        public override bool ClosedDomain => true;

        float tick = 0;
        float symbolRotation = 0f;
        float blackFade = 0f;
        float bgScale = 0f;
        float symbolScale = 0f;
        Texture2D symbolTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/NPCDomains/PhantasmicLabyrinthSymbol", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        Dictionary<int, List<int>> dragonMap = new Dictionary<int, List<int>>();
        bool spawnSwitch = false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawInnerDomain(() =>
            {
                Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

                spriteBatch.Draw(whiteTexture, screenRectangle, new Color(0, 0, 0, blackFade));

                if (tick > 90)
                {
                    Rectangle domainTextureSrc = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
                    spriteBatch.Draw(DomainTexture, center - Main.screenPosition, domainTextureSrc, Color.White, 0f, domainTextureSrc.Size() * 0.5f, bgScale * 2, SpriteEffects.None, 0f);

                    Rectangle symbolSrc = new Rectangle(0, 0, symbolTexture.Width, symbolTexture.Height);
                    spriteBatch.Draw(symbolTexture, center - Main.screenPosition, symbolSrc, Color.White, symbolRotation, symbolSrc.Size() * 0.5f, symbolScale * 2, SpriteEffects.None, 0f);
                }
            },

            () => spriteBatch.Draw(BaseTexture, center - Main.screenPosition, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height), Color.White, 0f, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height).Size() * 0.5f, 2f, SpriteEffects.None, 0f)
            );

        }

        public override void SureHitEffect(Player player)
        {
            if (tick < 150 && (Main.netMode == NetmodeID.SinglePlayer || Main.netMode == NetmodeID.Server)) return;

            float spawnTick = tick - 150f;

            if (spawnTick % 60 == 0)
            {
                if (!dragonMap.ContainsKey(player.whoAmI))
                {

                    Vector2[] offsets = spawnSwitch ?
                    [
                            new Vector2(-400, -400),
                            new Vector2(400, -400),
                            new Vector2(-400, 400),
                            new Vector2(400, 400),
                    ]
                    :
                    [
                            new Vector2(-600, 0),
                            new Vector2(600, 0),
                            new Vector2(0, 600),
                            new Vector2(0, -600),
                    ];

                    spawnSwitch = !spawnSwitch;

                    List<int> spawnedDragons = new List<int>();

                    for (int i = 0; i < offsets.Length; i++)
                    {
                        Vector2 pos = player.Center + offsets[i];
                        int index = NPC.NewNPC(null, (int)pos.X, (int)pos.Y, NPCID.CultistDragonHead);

                        if (index >= 0 && Main.npc[index].active)
                        {
                            NPC drag = Main.npc[index];
                            drag.target = player.whoAmI;
                            drag.velocity = (player.Center - pos).SafeNormalize(Vector2.UnitX) * 10f;
                            drag.noTileCollide = true;
                            drag.netUpdate = true;

                            spawnedDragons.Add(index);
                        }
                    }

                    dragonMap.Add(player.whoAmI, spawnedDragons);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        if (dragonMap.TryGetValue(player.whoAmI, out var dragList))
                        {
                            foreach (int dragWhoAmI in dragList)
                            {
                                Main.npc[dragWhoAmI].active = false;
                                Main.npc[dragWhoAmI].netUpdate = true;
                            }
                        }

                        dragonMap.Remove(player.whoAmI);
                    }, 60);
                }
            }

            if (dragonMap.TryGetValue(player.whoAmI, out var dragonList))
            {
                foreach (int index in dragonList)
                {
                    if (Main.npc[index].active)
                    {
                        NPC drag = Main.npc[index];
                        drag.target = player.whoAmI;
                        drag.velocity = (player.Center - drag.Center).SafeNormalize(Vector2.UnitX) * 10f;
                        drag.noTileCollide = true;
                        drag.netUpdate = true;
                    }
                }
            }
        }

        public override void Update()
        {
            base.Update();

            tick++;

            if (tick < 240)
                Main.npc[owner].Center = center;

            if ((blackFade += 0.011f) > 1f)
                blackFade = 1f;

            if ((symbolRotation += 0.01f) > MathF.PI * 2f)
                symbolRotation = 0;

            if (tick > 90)
            {
                float symDist = 1f - symbolScale;
                if (symDist > 0.01f)
                {
                    symbolScale += (1f - symbolScale) / 5f;
                }
            }

            if (tick > 150)
            {
                float bgDist = 1f - bgScale;
                if (bgDist > 0.01f)
                {
                    bgScale += (1f - bgScale) / 5f;
                }
            }
        }

        public override void CloseDomain()
        {
            tick = 0;
            symbolRotation = 0f;
            blackFade = 0f;
            bgScale = 0f;
            symbolScale = 0f;
            Dictionary<int, List<int>> dragonMap = new Dictionary<int, List<int>>();
            spawnSwitch = false;
        }
    }
}
