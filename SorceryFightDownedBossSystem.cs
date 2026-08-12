using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{
    public class SorceryFightDownedBossSystem : ModSystem
    {
        private static bool _downedMahoraga = false;

        public static bool downedMahoraga
        {
            get => _downedMahoraga;
            set
            {
                if (!value)
                    _downedMahoraga = false;
                else
                    NPC.SetEventFlagCleared(ref _downedMahoraga, -1);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();

            downed.AddWithCondition("Mahoraga", downedMahoraga);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downedFlags");
            
            downedMahoraga = downed.Contains("Mahoraga");
        }
    }
}