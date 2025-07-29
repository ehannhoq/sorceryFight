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
                Filters.Scene["SF:HollowNuke"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(hollowNukeCollision, "WhiteFade"), EffectPriority.VeryHigh);
                Filters.Scene["SF:HollowNuke"].Load();

                Asset<Effect> maximumRedSpawn = Mod.Assets.Request<Effect>("Content/Shaders/MaximumRed", AssetRequestMode.ImmediateLoad);
                Filters.Scene["SF:MaximumRed"] = new Filter(new Terraria.Graphics.Shaders.ScreenShaderData(maximumRedSpawn, "Desaturate"), EffectPriority.VeryHigh);
                Filters.Scene["SF:MaximumRed"].Load();

            }
        }
    }
} 