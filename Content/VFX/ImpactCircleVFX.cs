using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Utilities.EaseFunctions;
using Terraria.ModLoader;

namespace sorceryFight.Content.VFX
{
    public class ImpactCircleVFX : VFXObject
    {
        float baseScale;
        public ImpactCircleVFX(Vector2 center, int lifetime, float scale = 1f, Color? color = null) : 
        base(
            ModContent.Request<Texture2D>("sorceryFight/Content/VFX/ImpactCircle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            center,
            lifetime: lifetime,
            scale: scale,
            color: color
        ) {
            baseScale = scale;
        }


        internal override void Update()
        {
            base.Update();

            scale = EaseFunctions.EaseOutCircular((float)tick / lifetime) * baseScale;
            opacity = 1 - EaseFunctions.EaseOutCircular((float)tick / lifetime);
        }
    }
}