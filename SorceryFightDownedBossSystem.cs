using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight
{
    public class SorceryFightDownedBossSystem : ModSystem
    {
        private static bool _downedFingerBearerI = false;
        private static bool _downedFingerBearerII = false;

        public static bool downedFingerBearerI
        {
            get => _downedFingerBearerI;
            set
            {
                if (!value)
                    _downedFingerBearerI = false;
                else
                    NPC.SetEventFlagCleared(ref _downedFingerBearerI, -1);
            }
        }

        public static bool downedFingerBearerII
        {
            get => _downedFingerBearerII;
            set
            {
                if (!value)
                    _downedFingerBearerII = false;
                else
                    NPC.SetEventFlagCleared(ref _downedFingerBearerII, -1);
            }
        }

        public override void SaveWorldData(TagCompound tag)
        {
            List<string> downed = new List<string>();

            downed.AddWithCondition("FingerBearerI", downedFingerBearerI);
            downed.AddWithCondition("FingerBearerII", downedFingerBearerII);
        }

        public override void LoadWorldData(TagCompound tag)
        {
            IList<string> downed = tag.GetList<string>("downedFlags");
            
            downedFingerBearerI = downed.Contains("FingerBearerI");
            downedFingerBearerII = downed.Contains("FingerBearerII");
        }
    }
}