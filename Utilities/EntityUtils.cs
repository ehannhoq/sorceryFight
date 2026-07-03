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

        public static bool IsHitboxTileColliding(Rectangle hitbox, out Vector2 hitPosition)
        {
            int left = hitbox.Left / 16;
            int right = hitbox.Right / 16;
            int top = hitbox.Top / 16;
            int bottom = hitbox.Bottom / 16;

            for (int x = left; x <= right; x++)
            {
                for (int y = top; y <= bottom; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                    {
                        hitPosition = new Vector2(x * 16, y * 16);
                        return true;
                    }

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (tile.HasTile && Main.tileSolid[tile.TileType])
                    {
                        hitPosition = new Vector2(x * 16, y * 16);
                        return true;
                    }
                }
            }

            hitPosition = Vector2.Zero;
            return false;
        }


        public static Vector2 AdjustDashPosition(Vector2 dashVector, Rectangle npcHitbox)
        {
            Rectangle futureHitbox = new Rectangle((int)(dashVector.X - npcHitbox.Width / 2f), (int)(dashVector.Y - npcHitbox.Height / 2f), npcHitbox.Width, npcHitbox.Height);
            if (IsHitboxTileColliding(futureHitbox, out Vector2 hitPosition))
            {
                return hitPosition - new Vector2(npcHitbox.Width / 2f, npcHitbox.Height / 2f);
            }
            return dashVector;
        }
    }
}
