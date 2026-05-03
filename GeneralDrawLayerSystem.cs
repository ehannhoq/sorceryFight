using sorceryFight.Content.Enums;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems.Graphic
{
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

        public override void OnModLoad()
        {
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
            Main.spriteBatch.End(out var ss);
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.BeforeAllTiles);
            Main.spriteBatch.Begin(ss);
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
            Main.spriteBatch.End(out var ss);
            GeneralDrawLayer_DrawForLayer(GeneralDrawLayer.AfterEverything);
            Main.spriteBatch.Begin(ss);
        }
        #endregion
    }
}
