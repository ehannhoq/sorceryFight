using System;
using System.Collections.Generic;
using CalamityMod;
using Ionic.Zip;
using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Limitless
{
    
    public class InfinityBuff : PassiveTechnique
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
        public override string LockedDescription
        {
            get
            {
                return Language.GetText("Mods.sorceryFight.PassiveTechniques.Infinity.LockedDescription").Value;
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 1f;
        public override bool Unlocked
        { 
            get
            {
                return NPC.downedBoss1; 
            }
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<InfinityBuff>(), 2);
      
            player.GetModPlayer<SorceryFightPlayer>().infinity = true;
        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            sf.infinity = false;
            sf.disableRegenFromBuffs = false; // Most likely temporary, up until there's another buff that requires disabling regen.
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            float projInfinityDistance = 100f;
            float npcInfinityDistance = 70f;
            CostPerSecond = 1f;

            sf.disableRegenFromBuffs = false;

            float accumulativeDamage = 0f;
            int numInInfinity = 0;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
   
                if (proj.hostile)
                {
                    float distance = Vector2.Distance(proj.Center, player.Center);
                    if (distance <= projInfinityDistance)
                    {
                        accumulativeDamage += proj.damage;
                        numInInfinity ++;

                        proj.velocity *= 0.5f;
                        Vector2 vector = player.Center.DirectionTo(proj.Center);
                        proj.velocity = vector * (3f + player.velocity.Length()) * ((projInfinityDistance - distance) / 75);
                    }
                }
            }
            
            foreach (NPC npc in Main.npc)
            {
                
                if (!npc.friendly && npc.type != NPCID.TargetDummy && npc.active && !SorceryFight.IsDomain(npc))
                {
                    float distance = Vector2.Distance(npc.Center, player.Center);
                    if (distance <= npcInfinityDistance)
                    {
                        accumulativeDamage += npc.damage;
                        numInInfinity ++;

                        npc.velocity *= 0.5f;
                        Vector2 vector = player.Center.DirectionTo(npc.Center);
                        npc.velocity = vector * (3f + player.velocity.Length()) * ((npcInfinityDistance - distance) / 50);
                    }
                }
            }


            if (accumulativeDamage > 0 || numInInfinity > 0)
            {
                sf.disableRegenFromBuffs = true;
            }

            accumulativeDamage -= player.statDefense;

            CostPerSecond += accumulativeDamage;

            base.Update(player, ref buffIndex);
        }
    }
}
