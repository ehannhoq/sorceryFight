using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class UnlimitedVoidBuff : ModBuff
    {
        public Dictionary<int, float[]> npcFrozenValues;
        public Dictionary<int, float[]> playerFrozenValues;

        public override void Load()
        {
            npcFrozenValues = new Dictionary<int, float[]>();
            playerFrozenValues = new Dictionary<int, float[]>();
        }

        public override void Unload()
        {
            npcFrozenValues = null;
            playerFrozenValues = null;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (!npcFrozenValues.ContainsKey(npc.whoAmI))
            {
                npcFrozenValues[npc.whoAmI] = new float[6];
                float[] vals = [npc.Center.X, npc.Center.Y, npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3]];
                Array.Copy(vals, npcFrozenValues[npc.whoAmI], 6);
            }

            Vector2 frozenPosition = new Vector2(npcFrozenValues[npc.whoAmI][0], npcFrozenValues[npc.whoAmI][1]);
            npc.Center = frozenPosition;

            npc.ai[0] = npcFrozenValues[npc.whoAmI][2];
            npc.ai[1] = npcFrozenValues[npc.whoAmI][3];
            npc.ai[2] = npcFrozenValues[npc.whoAmI][4];
            npc.ai[3] = npcFrozenValues[npc.whoAmI][5];
        }
    }
}
