using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public class DrawMoonLordAboveDE : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == NPCID.MoonLordHead || entity.type == NPCID.MoonLordHand || entity.type == NPCID.MoonLordCore;

        public override void DrawBehind(NPC npc, int index)
        {
            npc.behindTiles = false;
            npc.hide = true;

            if (!Main.instance.DrawCacheNPCsMoonMoon.Contains(index))
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

            if (Main.instance.DrawCacheNPCProjectiles.Contains(index))
            {
                Main.instance.DrawCacheNPCProjectiles.Remove(index);
            }


            if (DomainExpansionController.ActiveDomains.Count > 0)
            {
                foreach (var kv in DomainExpansionController.ActiveDomains)
                {
                    var de = kv.Value;
                    if (Vector2.DistanceSquared(de.center, npc.Center) < de.SureHitRange.Squared())
                    {
                        npc.behindTiles = true;
                        Main.instance.DrawCacheNPCsMoonMoon.Remove(index);
                        Main.instance.DrawCacheNPCProjectiles.Add(index);
                    }
                }
            }
        }
    }
}
