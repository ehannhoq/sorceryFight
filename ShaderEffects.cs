using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;

namespace sorceryFight
{
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
            }
        }
    }
}