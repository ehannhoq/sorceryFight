using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content
{
    public class RCTGranter : ModSystem
    {
        public static int planteraIndex = -1;
        private static Vector2 planteraPos;

        public override void PreUpdatePlayers()
        {
            if (!CheckPlantera()) return;

            foreach (Player player in Main.player)
            {
                if (!player.active || player == null) continue;

                SorceryFightPlayer sfPlayer = player.SorceryFight();

                if (sfPlayer.unlockedRCT || sfPlayer.rctAnimation) continue;

                sfPlayer.preventDeath = true;
            }
        }


        public static bool CheckPlantera()
        {
            int planteraType = NPCID.Plantera;
            if (planteraIndex >= 0 && Main.npc[planteraIndex].active && Main.npc[planteraIndex].type == planteraType)
                return true;


            planteraIndex = -1;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == planteraType)
                {
                    planteraIndex = n.whoAmI;
                    break;
                }
            }

            return planteraIndex != -1;
        }


        public override void PreUpdateNPCs()
        {
            if (planteraIndex == -1) return;

            NPC plantera = Main.npc[planteraIndex];

            HashSet<int> viableTargets = [];
            foreach (Player player in Main.ActivePlayers)
            {
                SorceryFightPlayer sfPlayer = player.SorceryFight();
                if (!sfPlayer.rctAnimation)
                    viableTargets.Add(player.whoAmI);
            }

            if (viableTargets.Count > 0)
                plantera.target = viableTargets.ElementAt(new Random().Next(viableTargets.Count));
            else
                plantera.velocity = Vector2.Zero;
        }
    }
}