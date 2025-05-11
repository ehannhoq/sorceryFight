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
    public class DrawNPCsAboveDE : GlobalNPC
    {

        public static Dictionary<int, NPCDrawData> npcData = new Dictionary<int, NPCDrawData>();

        public override void DrawBehind(NPC npc, int index)
        {
            if (DomainExpansionController.ActiveDomains.Count > 0)
            {
                foreach (var kv in DomainExpansionController.ActiveDomains)
                {
                    var de = kv.Value;
                    if (!de.ClosedDomain) continue;
                    if (Vector2.DistanceSquared(de.center, npc.Center) < de.SureHitRange.Squared())
                    {
                        if (!npcData.ContainsKey(npc.whoAmI))
                        {
                            npcData[npc.whoAmI] = GrabNPCDrawData(npc, index);
                            return;
                        }

                        ClearCacheNPCs(index);

                        npc.hide = true;
                        npc.behindTiles = false;

                        Main.instance.DrawCacheNPCProjectiles.Add(index);

                        return;
                    }
                }
            }

            if (npcData.ContainsKey(npc.whoAmI))
            {
                NPCDrawData data = npcData[npc.whoAmI];
                npc.hide = data.Hide;
                npc.behindTiles = data.BehindTiles;

                if (data.MoonMoon)
                    Main.instance.DrawCacheNPCsMoonMoon.Insert(data.LayerIndex, index);

                if (data.BehindNonSolidTiles)
                    Main.instance.DrawCacheNPCsBehindNonSolidTiles.Insert(data.LayerIndex, index);

                if (data.Projectiles)
                    Main.instance.DrawCacheNPCProjectiles.Insert(data.LayerIndex, index);

                if (data.OverPlayers)
                    Main.instance.DrawCacheNPCsOverPlayers.Insert(data.LayerIndex, index);

                npcData.Remove(npc.whoAmI);
            }
        }

        public void ClearCacheNPCs(int index)
        {
            if (Main.instance.DrawCacheNPCsMoonMoon.Contains(index))
            {
                Main.instance.DrawCacheNPCsMoonMoon.Remove(index);
            }

            if (Main.instance.DrawCacheNPCsBehindNonSolidTiles.Contains(index))
            {
                Main.instance.DrawCacheNPCsBehindNonSolidTiles.Remove(index);
            }

            if (Main.instance.DrawCacheNPCProjectiles.Contains(index))
            {
                Main.instance.DrawCacheNPCProjectiles.Remove(index);
            }

            if (Main.instance.DrawCacheNPCsOverPlayers.Contains(index))
            {
                Main.instance.DrawCacheNPCsOverPlayers.Remove(index);
            }
        }

        public NPCDrawData GrabNPCDrawData(NPC npc, int index)
        {
            NPCDrawData data = new NPCDrawData();
            data.Hide = npc.hide;
            data.BehindTiles = npc.behindTiles;
            data.MoonMoon = Main.instance.DrawCacheNPCsMoonMoon.Contains(index);
            data.BehindNonSolidTiles = Main.instance.DrawCacheNPCsBehindNonSolidTiles.Contains(index);
            data.Projectiles = Main.instance.DrawCacheNPCProjectiles.Contains(index);
            data.OverPlayers = Main.instance.DrawCacheNPCsOverPlayers.Contains(index);

            return data;

        }


        public struct NPCDrawData
        {
            public bool Hide;
            public bool BehindTiles;
            public int LayerIndex;
            public bool MoonMoon;
            public bool BehindNonSolidTiles;
            public bool Projectiles;
            public bool OverPlayers;
        }
    }
}
