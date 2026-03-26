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


        [BackgroundColor(54, 192, 220, 192)]
        [DefaultValue(true)]
        public bool CursedEnergyBar { get; set; }

        [BackgroundColor(54, 192, 220, 192)]
        [DefaultValue(true)]
        public bool CursedEnergyBarPosLock { get; set; }

        [BackgroundColor(54, 192, 220, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(CursedEnergyUI.DefaultCursedBarPosX)]
        public float CursedEnergyBarPosX { get; set; }

        [BackgroundColor(54, 192, 220, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(CursedEnergyUI.DefaultCursedBarPosY)]
        public float CursedEnergyBarPosY { get; set; }

        [BackgroundColor(54, 192, 220, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        public float CursedEnergyBarTransparency { get; set; }


        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool BloodEnergyBar { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool BloodEnergyBarPosLock { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(BloodEnergyUI.DefaultBloodBarPosX)]
        public float BloodEnergyBarPosX { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(BloodEnergyUI.DefaultBloodBarPosY)]
        public float BloodEnergyBarPosY { get; set; }

        [BackgroundColor(192, 54, 64, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        public float BloodEnergyBarTransparency { get; set; }

        [BackgroundColor(128, 0, 192, 192)]
        [DefaultValue(true)]
        public bool StarMeter { get; set; }

        [BackgroundColor(128, 0, 192, 192)]
        [DefaultValue(true)]
        public bool StarMeterPosLock { get; set; }

        [BackgroundColor(128, 0, 192, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StarUI.DefaultStarPosX)]
        public float StarMeterPosX { get; set; }

        [BackgroundColor(128, 0, 192, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 100f)]
        [DefaultValue(StarUI.DefaultStarPosY)]
        public float StarMeterPosY { get; set; }

        [BackgroundColor(128, 0, 192, 192)]
        [SliderColor(224, 165, 56, 128)]
        [Range(0f, 1f)]
        [DefaultValue(1f)]
        public float StarMeterTransparency { get; set; }


    }
}
