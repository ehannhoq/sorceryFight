using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.PassiveTechniques.Limitless
{
    
    public class Infinity : PassiveTechnique
    {
        public override string Name { get; set; } = "Infinity";
        public override string Stats 
        {
            get
            {
                return $"Base CE Consumption: 1 CE/s\n"
                        + "Each object blocked by Infinity\n"
                        + "increases CE consumption dependent\n"
                        + "on the object's damamge.\n";
                        
            }
        }
        public override LocalizedText Description => Language.GetText("Mods.sorceryFight.PassiveTechniques.Infinity.Description");
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 1f;

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<Infinity>(), 2);
            player.GetModPlayer<SorceryFightPlayer>().infinity = true;
        }

        public override void Remove(Player player)
        {
            player.GetModPlayer<SorceryFightPlayer>().infinity = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            float projInfinityDistance = 100f;
            float npcInfinityDistance = 70f;
            CostPerSecond = 1f;
            float accumulativeDamage = 0f;

            foreach (Projectile proj in Main.projectile)
            {
   
                if (proj.hostile)
                {
                    float distance = Vector2.Distance(proj.Center, player.Center);
                    if (distance <= projInfinityDistance)
                    {
                        accumulativeDamage += proj.damage;

                        proj.velocity *= 0.5f;
                        Vector2 vector = player.Center.DirectionTo(proj.Center);
                        proj.velocity = vector * (3f + player.velocity.Length()) * ((projInfinityDistance - distance) / 75);
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
                        accumulativeDamage += npc.damage;
                        npc.velocity *= 0.5f;
            
                        Vector2 vector = player.Center.DirectionTo(npc.Center);
                        npc.velocity = vector * (3f + player.velocity.Length()) * ((npcInfinityDistance - distance) / 50);
                    }
                }
            }

            CostPerSecond += 3 * accumulativeDamage / (float)Math.Sqrt(accumulativeDamage + 1);

            base.Update(player, ref buffIndex);
        }
    }
}
