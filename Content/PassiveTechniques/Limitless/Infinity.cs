using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;

namespace sorceryFight.Content.PassiveTechniques.Limitless
{
    
    public class Infinity : PassiveTechnique
    {
        public override float CostPerTick { get; set; } = 1f;

        public override void Update(Player player, ref int buffIndex)
        {
            float projInfinityDistance = 100f;
            float npcInfinityDistance = 70f;

            foreach (Projectile proj in Main.projectile)
            {
                if (proj.hostile)
                {
                    float distance = Vector2.Distance(proj.Center, player.Center);
                    if (distance <= projInfinityDistance)
                    {
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
                if (!npc.friendly)
                {
                    float distance = Vector2.Distance(npc.Center, player.Center);
                    if (distance <= npcInfinityDistance)
                    {
                        npc.ai[3] += 1f;
                        npc.velocity *= 0.5f;

            
                        Vector2 vector = player.Center.DirectionTo(npc.Center);
                        npc.velocity = vector * (3f + player.velocity.Length()) * ((npcInfinityDistance - distance) / 50);
                    }
                }
            }

            base.Update(player, ref buffIndex);
        }
    }
}
