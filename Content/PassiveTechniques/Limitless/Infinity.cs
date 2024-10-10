using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.PassiveTechniques.Limitless
{
    
    public class Infinity : PassiveTechnique
    {
        public override float CostPerTick { get; set; } = 0.1f;
        public int numInInfinity = 0;


        public override void Update(Player player, ref int buffIndex)
        {
            float projInfinityDistance = 100f;
            float npcInfinityDistance = 70f;
            numInInfinity = 0;
            CostPerTick = 0.1f;

            foreach (Projectile proj in Main.projectile)
            {
   
                if (proj.hostile)
                {
                    float distance = Vector2.Distance(proj.Center, player.Center);
                    if (distance <= projInfinityDistance)
                    {
                        numInInfinity ++;
                        proj.velocity *= 0.5f;

                        proj.ai[2] += 1;

    
                        Vector2 vector = player.Center.DirectionTo(proj.Center);
                        proj.velocity = vector * (3f + player.velocity.Length()) * ((projInfinityDistance - distance) / 75);


                        if (proj.ai[2] >= 60)
                            proj.Kill();
                    }
                }
            }
    
            foreach (NPC npc in Main.npc)
            {
                if (!npc.friendly && npc.type != NPCID.TargetDummy)
                {
                    float distance = Vector2.Distance(npc.Center, player.Center);
                    if (distance <= npcInfinityDistance)
                    {
                        numInInfinity ++;
                        npc.ai[3] += 1f;
                        npc.velocity *= 0.5f;

            
                        Vector2 vector = player.Center.DirectionTo(npc.Center);
                        npc.velocity = vector * (3f + player.velocity.Length()) * ((npcInfinityDistance - distance) / 50);
                    }
                }
            }
            
            CostPerTick += (float)numInInfinity / 10;

            base.Update(player, ref buffIndex);
        }
    }
}
