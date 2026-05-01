using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;

namespace sorceryFight
{
    public static partial class SFUtils
    {
        /// <summary>
        /// Check if Entity is null or Inactive (!active)
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <returns>true if entity is null or inactive, otherwise false</returns>
        public static bool IsNullOrInactive(this Entity entity)
        {
            if (entity is null) return true;
            if (!entity.active) return true;

            return false;
        }


        //add enemies weak to RCT here
        //only these NPCs get damaged by RCT things
        private static readonly HashSet<int> RCTWeakNPC = new()
        {
            // Blood Moon
            NPCID.BloodZombie,
            NPCID.Drippler,
            NPCID.TheGroom,
            NPCID.TheBride,
            NPCID.EyeballFlyingFish,
            NPCID.ZombieMerman,
            NPCID.GoblinShark,
            NPCID.BloodEelHead,
            NPCID.BloodEelBody,
            NPCID.BloodEelTail,
            NPCID.BloodNautilus,
            NPCID.BloodSquid,
            NPCID.Clown,
            NPCID.ChatteringTeethBomb,

            // Blood Moon critters
            NPCID.CorruptBunny,
            NPCID.CrimsonBunny,
            NPCID.CorruptGoldfish,
            NPCID.CrimsonGoldfish,
            NPCID.CorruptPenguin,
            NPCID.CrimsonPenguin,

            // Corruption surface
            NPCID.EaterofSouls,
            NPCID.BigEater,
            NPCID.LittleEater,
            NPCID.DevourerHead,
            NPCID.DevourerBody,
            NPCID.DevourerTail,
            NPCID.CorruptSlime,
            NPCID.Slimeling,
            NPCID.Slimer,
            NPCID.Slimer2,
            NPCID.ShadowFlameApparition,

            // Corruption underground
            NPCID.SeekerHead,
            NPCID.SeekerBody,
            NPCID.SeekerTail,
            NPCID.CursedHammer,
            NPCID.Clinger,
            NPCID.Corruptor,
            //NPCID.CorruptGoldBunny,
            //NPCID.CorruptGoldGoldfish,

            // Crimson surface
            NPCID.FaceMonster,
            NPCID.Crimera,
            NPCID.BigCrimera,
            NPCID.LittleCrimera,
            NPCID.BloodCrawler,
            NPCID.BloodCrawlerWall,
            NPCID.CrimsonAxe,

            // Crimson underground
            NPCID.Herpling,
            NPCID.Crimslime,
            NPCID.IchorSticker,
            NPCID.FloatyGross,
            NPCID.BloodJelly,
            NPCID.BloodFeeder,
            //NPCID.CrimsonGoldBunny,
            //NPCID.CrimsonGoldGoldfish,

            // Hardmode corruption/crimson desert
            NPCID.DarkMummy,
            NPCID.BloodMummy,
            //NPCID.ShadowHammer,

            // Corruptor spit
            //NPCID.CorruptorVileSpit,

            // Hallow/corruption crossover
            //NPCID.Pigron,
            NPCID.PigronCorruption,
            NPCID.PigronCrimson,

            // Zombies
            NPCID.Zombie,
            NPCID.BaldZombie,
            NPCID.PincushionZombie,
            NPCID.SlimedZombie,
            NPCID.SwampZombie,
            NPCID.TwiggyZombie,
            NPCID.FemaleZombie,
            NPCID.ZombieRaincoat,
            NPCID.ZombieEskimo,
            NPCID.ArmedZombie,
            NPCID.ArmedZombieSlimed,
            NPCID.ArmedZombieSwamp,
            NPCID.ArmedZombieTwiggy,
            NPCID.ArmedZombieCenx,
            NPCID.ArmedZombieEskimo,
            NPCID.MaggotZombie,

            // Demon Eyes
            NPCID.DemonEye,
            NPCID.CataractEye,
            NPCID.SleepyEye,
            NPCID.DialatedEye,
            NPCID.GreenEye,
            NPCID.PurpleEye,
            NPCID.DemonEyeOwl,
            NPCID.DemonEyeSpaceship

        };

        public static bool IsRCTWeakNPC(NPC npc) => RCTWeakNPC.Contains(npc.type);



    }
}
