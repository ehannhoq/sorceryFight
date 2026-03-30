using sorceryFight.Content.UI.TechniqueSelector;
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

        #region CursedTechniqueSelector
        [Header("CursedTechniqueSelector")]
        [Label("Cursed Technique Selector X Position")]
        [Range(0f, 100f)]
        [DefaultValue(CursedTechniqueSelector.DefaultCTSelectorPosX)]
        public float CTSelectorPosX { get; set; } = 46.875f;

        [Label("Cursed Technique Selector Y Position")]
        [Range(0f, 100f)]
        [DefaultValue(CursedTechniqueSelector.DefaultCTSelectorPosX)]
        public float CTSelectorPosY { get; set; } = 90.789f;

        [Label("Lock Cursed Technique Selector Position")]
        [DefaultValue(false)]
        public bool CTSelectorPosLock { get; set; } = false;
        #endregion

        #region Energy Bars
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

        [Header("BloodEnergyBar")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool BloodEnergyBar { get; set; }

        [Label("Blood Energy Bar Scale")]
        [Tooltip("Adjusts the size of the Blood Energy Bar. Default is 1.0")]
        [Range(0.5f, 3.0f)]
        [Increment(0.05f)]
        [DefaultValue(1f)]
        public float BloodEnergyBarScale { get; set; } = 1f;

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
        #endregion

    }
}
