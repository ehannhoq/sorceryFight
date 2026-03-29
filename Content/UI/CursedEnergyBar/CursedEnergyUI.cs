using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using sorceryFight;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

public class CursedEnergyUI : ModSystem
{

    //All based on the stealth UI

    SorceryFightPlayer sfPlayer;
    public UIImage border;
    public ValueBar beBarValue;
    private bool initialized;
    bool hasRightClicked;
    Vector2 offset;
    Texture2D borderTexture;

    internal const float DefaultCursedBarPosX = 78.242188f;
    internal const float DefaultCursedBarPosY = 12.645349f;
    private const float MouseDragEpsilon = 0.05f; // 0.05%

    private static Vector2? dragOffset = null;
    private static Texture2D edgeTexture, barTexture;

    public override void OnModLoad()
    {
        edgeTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarBorder", AssetRequestMode.ImmediateLoad).Value;
        barTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarFill", AssetRequestMode.ImmediateLoad).Value;
        Reset();
    }

    public override void Unload()
    {
        Reset();
        edgeTexture = barTexture = null;
    }

    private static void Reset() => dragOffset = null;

    public static void Draw(SpriteBatch spriteBatch, Player player)
    {
        // Sanity check the planned position before drawing
        Vector2 screenRatioPosition = new Vector2(ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosX, ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosY);
        if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
            screenRatioPosition.X = DefaultCursedBarPosX;
        if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
            screenRatioPosition.Y = DefaultCursedBarPosY;

        // Convert the screen ratio position to an absolute position in pixels
        // Cast to integer to prevent blurriness which results from decimal pixel positions
        float uiScale = Main.UIScale;
        Vector2 screenPos = screenRatioPosition;
        screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
        screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

        SorceryFightPlayer sf = Main.LocalPlayer.SorceryFight();

        // If not drawing the cursed meter, save its latest position to config and leave.
        if (sf.innateTechnique != null)
        {
            DrawCursedEnergyBar(spriteBatch, sf, screenPos);
        }
        else
        {
            bool changed = false;
            if (ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosX != screenRatioPosition.X)
            {
                ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosX = screenRatioPosition.X;
                changed = true;
            }
            if (ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosY != screenRatioPosition.Y)
            {
                ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosY = screenRatioPosition.Y;
                ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosY = screenRatioPosition.Y;
                changed = true;
            }

            if (changed)
                ModContent.GetInstance<ClientConfig>().SaveChanges();
        }

        Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
        Rectangle cursedEnergyBar = Utils.CenteredRectangle(screenPos, edgeTexture.Size() * uiScale);

        MouseState ms = Mouse.GetState();
        Vector2 mousePos = Main.MouseScreen;

        // Handle mouse dragging
        if (cursedEnergyBar.Intersects(mouseHitbox))
        {
            if (!ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosLock)
                Main.LocalPlayer.mouseInterface = true;

            // If the mouse is on top of the meter, show the player's exact numeric cursed power
            if (sf.maxCursedEnergy > 0f)
            {
                Main.hoverItemName = $"{SFUtils.GetLocalizationValue($"Mods.sorceryFight.UI.CursedEnergyBar.CE")} {Math.Round((decimal)sf.cursedEnergy, 0)} / {sf.maxCursedEnergy}\n"
                                    + $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.CursedEnergyBar.RegenRate")} {sf.cursedEnergyRegenPerSecond} CE/s\n"
                                    + SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.CursedEnergyBar.ToolTip");
            }

            Vector2 newScreenRatioPosition = screenRatioPosition;
            // As long as the mouse button is held down, drag the meter along with an offset.
            if (!ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosLock && ms.LeftButton == ButtonState.Pressed)
            {
                // If the drag offset doesn't exist yet, create it.
                if (!dragOffset.HasValue)
                    dragOffset = mousePos - screenPos;

                // Given the mouse's absolute current position, compute where the corner of the stealth bar should be based on the original drag offset.
                Vector2 newCorner = mousePos - dragOffset.GetValueOrDefault(Vector2.Zero);

                // Convert the new corner position into a screen ratio position.
                newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
            }

            // Compute the change in position. If it is large enough, actually move the meter
            Vector2 delta = newScreenRatioPosition - screenRatioPosition;
            if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
            {
                ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosX = newScreenRatioPosition.X;
                ModContent.GetInstance<ClientConfig>().CursedEnergyBarPosY = newScreenRatioPosition.Y;
            }

            // When the mouse is released, save the config and destroy the drag offset.
            if (dragOffset.HasValue && ms.LeftButton == ButtonState.Released)
            {
                dragOffset = null;
                ModContent.GetInstance<ClientConfig>().SaveChanges();
            }
        }
    }

    private static void DrawCursedEnergyBar(SpriteBatch spriteBatch, SorceryFightPlayer sf, Vector2 screenPos)
    {
        float uiScale = Main.UIScale;
        float transparency = ModContent.GetInstance<ClientConfig>().CursedEnergyBarTransparency;
        float offset = (edgeTexture.Width - barTexture.Width) * 0.5f;

        spriteBatch.Draw(edgeTexture, screenPos, null, Color.White * transparency, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

        float completionRatio = sf.maxCursedEnergy <= 0f ? 0f : sf.cursedEnergy / sf.maxCursedEnergy;
        Texture2D activeBar = barTexture;

        if(sf.innateTechnique.Name == "HeavenlyRestriction")
        {
            activeBar = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedEnergyBar/StaminaBarFill", AssetRequestMode.ImmediateLoad).Value;
        }

        int filledHeight = (int)(barTexture.Height * completionRatio);
        if (filledHeight <= 0) return;

        Rectangle barRectangle = new Rectangle(0, barTexture.Height - filledHeight, barTexture.Width, filledHeight);

        // Draw with no origin, manually positioning so the bar bottom stays fixed
        float unfilledHeight = barTexture.Height - filledHeight;
        Vector2 barPos = screenPos + new Vector2(offset - barTexture.Width * 0.5f, -barTexture.Height * 0.5f + unfilledHeight) * uiScale;

        spriteBatch.Draw(activeBar, barPos, barRectangle, Color.White * transparency, 0f, Vector2.Zero, uiScale, SpriteEffects.None, 0);
    }

    //This actually calls the cursed UI, maybe create another file to do it if this is rolled out for other UIs, Reference Calamity UIManagementSystem.cs
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        if (mouseIndex != -1)
        {
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Cursed UI", () =>
            {
                Draw(Main.spriteBatch, Main.LocalPlayer);
                return true;
            }, InterfaceScaleType.None));
        }
    }

}