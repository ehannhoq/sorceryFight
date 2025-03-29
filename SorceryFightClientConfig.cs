using System;
using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace sorceryFight
{
    public class ClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [DefaultValue(true)]
        public bool BlackFlashScreenEffects { get; set; }
    }
}
