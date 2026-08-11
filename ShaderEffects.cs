using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace sorceryFight
{
    [Autoload(Side = ModSide.Client)]
    public class ShaderEffects : ModSystem
    {
        public override void Load()
        {
            if (!Main.dedServ)
            {
                Asset<Effect> hollowNukeCollision = Mod.Assets.Request<Effect>("Content/Shaders/HollowNukeCollision", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:HollowNuke"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(hollowNukeCollision, "WhiteFade"), EffectPriority.Medium);
                Filters.Scene["SF:HollowNuke"].Load();

                Asset<Effect> maximumRedSpawn = Mod.Assets.Request<Effect>("Content/Shaders/MaximumRed", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:MaximumRed"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(maximumRedSpawn, "Desaturate"), EffectPriority.Medium);
                Filters.Scene["SF:MaximumRed"].Load();

                Asset<Effect> divineFlameMS = Mod.Assets.Request<Effect>("Content/Shaders/DivineFlame", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:DivineFlame"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(divineFlameMS, "OrangeTint"), EffectPriority.Medium);
                Filters.Scene["SF:DivineFlame"].Load();

                Asset<Effect> impactFrame = Mod.Assets.Request<Effect>("Content/Shaders/ImpactFrame", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:ImpactFrame"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(impactFrame, "ImpactFrame"), EffectPriority.High);
                Filters.Scene["SF:ImpactFrame"].Load();

                Asset<Effect> blackHole = Mod.Assets.Request<Effect>("Content/Shaders/Blackhole", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:Blackhole"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(blackHole, "Blackhole"), EffectPriority.Medium);
                Filters.Scene["SF:Blackhole"].Load();


                Asset<Effect> mindlessCarnage = Mod.Assets.Request<Effect>("Content/Shaders/MindlessCarnage", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:MindlessCarnage"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(mindlessCarnage, "MindlessCarnage"), EffectPriority.Medium);
                Filters.Scene["SF:MindlessCarnage"].Load();


                Asset<Effect> worldCuttingSlash = Mod.Assets.Request<Effect>("Content/Shaders/WorldCuttingSlash", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:WorldCuttingSlash"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(worldCuttingSlash, "WorldCuttingSlash"), EffectPriority.Medium);
                Filters.Scene["SF:WorldCuttingSlash"].Load();

                Asset<Effect> limitlessRCTFilter = Mod.Assets.Request<Effect>("Content/Shaders/LimitlessRCTFilter", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:LimitlessRCTFilter"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(limitlessRCTFilter, "LimitlessRCTFilter"), EffectPriority.Medium);
                Filters.Scene["SF:LimitlessRCTFilter"].Load();

                Asset<Effect> blackScreen = Mod.Assets.Request<Effect>("Content/Shaders/BlackScreen", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:BlackScreen"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(blackScreen, "BlackScreen"), EffectPriority.High);
                Filters.Scene["SF:BlackScreen"].Load();
            }
        }
    }
}