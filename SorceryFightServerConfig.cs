using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace sorceryFight
{
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool LoreAccurateBlackFlash { get; set; }

        [DefaultValue(false)]
        public bool HollowNukeDamagesFriendlyNPCs { get; set; }
    }
}
