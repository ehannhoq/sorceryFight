using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Terraria.ModLoader.Config;

namespace sorceryFight
{
    public class ServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [DefaultValue(false)]
        public bool LoreAccurateBlackFlash { get; set; }
    }
}
