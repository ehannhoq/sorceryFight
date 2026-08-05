using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Utilities.EaseFunctions;
using Terraria.ModLoader;

namespace sorceryFight.Content.VFX
{
    public class ImpactRingVFX : VFXObject
    {
    float baseScale;
        public ImpactRingVFX(Vector2 center, int lifetime, float rotation = 0f, float scale = 1f, Color? color = null) : 
        base(
            ModContent.Request<Texture2D>("sorceryFight/Content/VFX/ImpactRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            center,
            lifetime: lifetime,
            scale: scale,
            color: color
        ) {
            baseScale = scale;
            base.rotation = rotation;
        }


        internal override void Update()
        {
            base.Update();

            scale = EaseFunctions.EaseOutCircular((float)tick / lifetime) * baseScale;
            opacity = 1 - EaseFunctions.EaseInCircular((float)tick / lifetime);
        }
    }
}