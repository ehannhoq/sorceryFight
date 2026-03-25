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
        [DefaultValue(false)]
        public bool DisableImpactFrames { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool StarMeter { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool MeterPosLock { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StarUI.DefaultStarPosX)]
        public float StarMeterPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StarUI.DefaultStarPosY)]
        public float StarMeterPosY { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        public float StarMeterTransparency { get; set; }


    }
}
