using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Enums;
using System;
using System.Reflection;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight
{

    //generated a summary with Claude, but feel free to edit it for accuracy and clarity.
    
    /// <summary>
    /// Hooks into Terraria's draw pipeline and fires events at each draw layer,
    /// allowing particles (and other systems) to draw at the correct point in
    /// the render order.
    /// If Calamity is also loaded, both mods hook the same <c>On_Main</c> events.
    /// MonoMod chains them via <c>orig</c> delegates, so both fire without conflict.
    /// Each mod's particle system draws independently at the correct layer.
    /// </summary>
    internal sealed class GeneralDrawLayerSystem : ModSystem
    {
        public static event Action OnPrepareDraw;
        public static event Action<GeneralDrawLayer> OnDrawLayer;
        public static event Action<GeneralDrawLayer> OnDrawLayerLate;

        public static event Action OnBeforeAllTiles;
        public static event Action OnBeforeSolidTiles;
        public static event Action OnBeforeNPCs;
        public static event Action OnAfterNPCs;
        public static event Action OnBeforeProjectiles;
        public static event Action OnAfterProjectiles;
        public static event Action OnAfterPlayers;
        public static event Action OnAfterDusts;
        public static event Action OnAfterEverything;

        // Cached reflection accessors for SpriteBatch's private fields.
        // These are private in FNA and not publicized by tModLoader.
        private static FieldInfo f_sortMode;
        private static FieldInfo f_blendState;
        private static FieldInfo f_samplerState;
        private static FieldInfo f_depthStencilState;
        private static FieldInfo f_rasterizerState;
        private static FieldInfo f_customEffect;
        private static FieldInfo f_transformMatrix;


        public override void OnModLoad()
        {
            // Cache reflection lookups once at load time.
            var sbType = typeof(SpriteBatch);
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
            f_sortMode = sbType.GetField("sortMode", flags) ?? sbType.GetField("_sortMode", flags);
            f_blendState = sbType.GetField("blendState", flags) ?? sbType.GetField("_blendState", flags);
            f_samplerState = sbType.GetField("samplerState", flags) ?? sbType.GetField("_samplerState", flags);
            f_depthStencilState = sbType.GetField("depthStencilState", flags) ?? sbType.GetField("_depthStencilState", flags);
            f_rasterizerState = sbType.GetField("rasterizerState", flags) ?? sbType.GetField("_rasterizerState", flags);
            f_customEffect = sbType.GetField("customEffect", flags) ?? sbType.GetField("_effect", flags);
            f_transformMatrix = sbType.GetField("transformMatrix", flags) ?? sbType.GetField("_matrix", flags);

            On_Main.CheckMonoliths += CheckMonoliths;
            On_Main.DrawBackgroundBlackFill += GeneralDrawLayer_DrawToLayer_BeforeAllTiles;
            On_Main.DoDraw_Tiles_Solid += GeneralDrawLayer_DrawToLayer_BeforeSolidTiles;
            On_Main.DoDraw_DrawNPCsOverTiles += GeneralDrawLayer_DrawToLayer_NPCs;
            On_Main.DrawProjectiles += GeneralDrawLayer_DrawToLayer_Projectiles;
            On_Main.DrawPlayers_AfterProjectiles += GeneralDrawLayer_DrawToLayer_AfterPlayers;
            On_Main.DrawDust += GeneralDrawLayer_DrawToLayer_AfterDusts;
            On_Main.DrawInfernoRings += GeneralDrawLayer_DrawToLayer_AfterEverything;
        }


        public override void Unload()
        {
            OnPrepareDraw = null;
            OnDrawLayer = null;
            OnDrawLayerLate = null;

            OnBeforeAllTiles = null;
            OnBeforeSolidTiles = null;
            OnBeforeNPCs = null;
            OnAfterNPCs = null;
            OnBeforeProjectiles = null;
            OnAfterProjectiles = null;
            OnAfterPlayers = null;
            OnAfterDusts = null;
            OnAfterEverything = null;
        }

        private static void CheckMonoliths(On_Main.orig_CheckMonoliths orig)
        {
            orig();
            OnPrepareDraw?.Invoke();
        }


        // Calamity uses Daybreak to do basically the same thing as this
        /// <summary>
        /// Snapshots the current SpriteBatch state, ends it, and restarts it
        /// with the same parameters after calling the provided action.
        /// This uses cached reflection to read the private fields.
        /// </summary>
        private static void EndAndRestart(SpriteBatch sb, Action drawAction)
        {
            var sortMode = (SpriteSortMode)f_sortMode.GetValue(sb);
            var blendState = (BlendState)f_blendState.GetValue(sb);
            var samplerState = (SamplerState)f_samplerState.GetValue(sb);
            var depthStencilState = (DepthStencilState)f_depthStencilState.GetValue(sb);
            var rasterizerState = (RasterizerState)f_rasterizerState.GetValue(sb);
            var customEffect = (Effect)f_customEffect.GetValue(sb);
            var transformMatrix = (Matrix)f_transformMatrix.GetValue(sb);

            sb.End();
            drawAction();
            sb.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, customEffect, transformMatrix);
        }

        #region GeneralDrawLayer Systems Drawing
        private static void GeneralDrawLayer_DrawForLayer(GeneralDrawLayer drawLayer)
        {
            OnDrawLayer?.Invoke(drawLayer);
            OnDrawLayerLate?.Invoke(drawLayer);

            var callback = drawLayer switch
            {
                GeneralDrawLayer.BeforeAllTiles => OnBeforeAllTiles,
                GeneralDrawLayer.BeforeSolidTiles => OnBeforeSolidTiles,
                GeneralDrawLayer.BeforeNPCs => OnBeforeNPCs,
                GeneralDrawLayer.AfterNPCs => OnAfterNPCs,
                GeneralDrawLayer.BeforeProjectiles => OnBeforeProjectiles,
                GeneralDrawLayer.AfterProjectiles => OnAfterProjectiles,
                GeneralDrawLayer.AfterPlayers => OnAfterPlayers,
                GeneralDrawLayer.AfterDusts => OnAfterDusts,
                GeneralDrawLayer.AfterEverything => OnAfterEverything,
                _ => null
            };
            callback?.Invoke();
        }

        private static void GeneralDrawLayer_DrawToLayer_BeforeAllTiles(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            EndAndRestart(Main.spriteBatch, () => GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.BeforeAllTiles));
            orig(self);
        }

        private static void GeneralDrawLayer_DrawToLayer_BeforeSolidTiles(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.BeforeSolidTiles);
            orig(self);
        }

        private static void GeneralDrawLayer_DrawToLayer_NPCs(On_Main.orig_DoDraw_DrawNPCsOverTiles orig, Main self)
        {
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.BeforeNPCs);
            orig(self);
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.AfterNPCs);
        }

        private static void GeneralDrawLayer_DrawToLayer_Projectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.BeforeProjectiles);
            orig(self);
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.AfterProjectiles);
        }

        private static void GeneralDrawLayer_DrawToLayer_AfterPlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig(self);
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.AfterPlayers);
        }

        private static void GeneralDrawLayer_DrawToLayer_AfterDusts(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.AfterDusts);
        }

        private static void GeneralDrawLayer_DrawToLayer_AfterEverything(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig(self);
            EndAndRestart(Main.spriteBatch, () => GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.AfterEverything));
        }
        #endregion
    }
}

