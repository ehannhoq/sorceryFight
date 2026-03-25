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

public class StarUI : ModSystem
{
    SorceryFightPlayer sfPlayer;
    public UIImage border;
    public ValueBar beBarValue;
    private bool initialized;
    bool hasRightClicked;
    Vector2 offset;
    Texture2D borderTexture;

    internal const float DefaultStarPosX = 50.304603f;
    internal const float DefaultStarPosY = 56.765408f;
    private const float MouseDragEpsilon = 0.05f; // 0.05%

    private static Vector2? dragOffset = null;
    private static Texture2D edgeTexture, barTexture, fullBarTexture;

    public override void OnModLoad()
    {
        edgeTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/StarUI/StarMeterBorder", AssetRequestMode.ImmediateLoad).Value;
        barTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/StarUI/StarMeterFill", AssetRequestMode.ImmediateLoad).Value;
        fullBarTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/StarUI/StarMeterFull", AssetRequestMode.ImmediateLoad).Value;
        Reset();
    }

    public override void Unload()
    {
        Reset();
        edgeTexture = barTexture = fullBarTexture = null;
    }

    private static void Reset() => dragOffset = null;

    public static void Draw(SpriteBatch spriteBatch, Player player)
    {
        // Sanity check the planned position before drawing
        Vector2 screenRatioPosition = new Vector2(ModContent.GetInstance<ClientConfig>().StarMeterPosX, ModContent.GetInstance<ClientConfig>().StarMeterPosY);
        if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
            screenRatioPosition.X = DefaultStarPosX;
        if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
            screenRatioPosition.Y = DefaultStarPosY;

        // Convert the screen ratio position to an absolute position in pixels
        // Cast to integer to prevent blurriness which results from decimal pixel positions
        float uiScale = Main.UIScale;
        Vector2 screenPos = screenRatioPosition;
        screenPos.X = (int)(screenPos.X * 0.01f * Main.screenWidth);
        screenPos.Y = (int)(screenPos.Y * 0.01f * Main.screenHeight);

        SorceryFightPlayer sf = Main.LocalPlayer.SorceryFight();

        // If not drawing the stealth meter, save its latest position to config and leave.
        if (sf.innateTechnique.Name == "StarRage")
        {
            DrawStarBar(spriteBatch, sf, screenPos);
        }
        else
        {
            bool changed = false;
            if (ModContent.GetInstance<ClientConfig>().StarMeterPosX != screenRatioPosition.X)
            {
                ModContent.GetInstance<ClientConfig>().StarMeterPosX = screenRatioPosition.X;
                changed = true;
            }
            if (ModContent.GetInstance<ClientConfig>().StarMeterPosY != screenRatioPosition.Y)
            {
                ModContent.GetInstance<ClientConfig>().StarMeterPosY = screenRatioPosition.Y;
                changed = true;
            }

            if (changed)
                ModContent.GetInstance<ClientConfig>().SaveChanges();
        }

        Rectangle mouseHitbox = new Rectangle((int)Main.MouseScreen.X, (int)Main.MouseScreen.Y, 8, 8);
        Rectangle starBar = Utils.CenteredRectangle(screenPos, edgeTexture.Size() * uiScale);

        MouseState ms = Mouse.GetState();
        Vector2 mousePos = Main.MouseScreen;

        // Handle mouse dragging
        if (starBar.Intersects(mouseHitbox))
        {
            if (!ModContent.GetInstance<ClientConfig>().MeterPosLock)
                Main.LocalPlayer.mouseInterface = true;

            // If the mouse is on top of the meter, show the player's exact numeric stealth.
            //if (sf.maxStarEnergy > 0f)
            //{
            //    string stealthStr = (100f * modPlayer.rogueStealth).ToString("n2");
            //    string maxStealthStr = (100f * modPlayer.rogueStealthMax).ToString("n2");
            //    string textToDisplay = $"{CalamityUtils.GetTextValue("UI.Stealth")}: {stealthStr}/{maxStealthStr}\n";

            //    if (!Main.keyState.PressingShift())
            //    {
            //        textToDisplay += CalamityUtils.GetTextValue("UI.StealthShiftText");
            //    }
            //    else
            //    {
            //        textToDisplay += CalamityUtils.GetTextValue("UI.StealthInfoText");
            //    }

            //    Main.instance.MouseText(textToDisplay, 0, 0, -1, -1, -1, -1);
            //    modPlayer.stealthUIAlpha = MathHelper.Lerp(modPlayer.stealthUIAlpha, 0.25f, 0.035f);
            //}

            Vector2 newScreenRatioPosition = screenRatioPosition;
            // As long as the mouse button is held down, drag the meter along with an offset.
            if (!ModContent.GetInstance<ClientConfig>().MeterPosLock && ms.LeftButton == ButtonState.Pressed)
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
                ModContent.GetInstance<ClientConfig>().StarMeterPosX = newScreenRatioPosition.X;
                ModContent.GetInstance<ClientConfig>().StarMeterPosY = newScreenRatioPosition.Y;
            }

            // When the mouse is released, save the config and destroy the drag offset.
            if (dragOffset.HasValue && ms.LeftButton == ButtonState.Released)
            {
                dragOffset = null;
                ModContent.GetInstance<ClientConfig>().SaveChanges();
            }
        }
    }

    private static void DrawStarBar(SpriteBatch spriteBatch, SorceryFightPlayer sf, Vector2 screenPos)
    {
        float uiScale = Main.UIScale;
        float offset = (edgeTexture.Width - barTexture.Width) * 0.5f;
        spriteBatch.Draw(edgeTexture, screenPos, null, Color.White * ModContent.GetInstance<ClientConfig>().StarMeterTransparency, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);

        float completionRatio = sf.maxStarEnergy <= 0f ? 0f : sf.starEnergy / sf.maxStarEnergy;
        Rectangle barRectangle = new Rectangle(0, 0, (int)(barTexture.Width * completionRatio), barTexture.Width);
        bool full = (sf.maxStarEnergy > 0f) && (sf.starEnergy >= sf.maxStarEnergy);
        spriteBatch.Draw(full ? fullBarTexture : barTexture, screenPos + new Vector2(offset * uiScale, 0), barRectangle, Color.White * ModContent.GetInstance<ClientConfig>().StarMeterTransparency, 0f, edgeTexture.Size() * 0.5f, uiScale, SpriteEffects.None, 0);
    }

    //This actually calls the star UI, maybe create another file to do it if this is rolled out for other UIs, Reference Calamity UIManagementSystem.cs
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        int mouseIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
        if (mouseIndex != -1)
        {
            layers.Insert(mouseIndex, new LegacyGameInterfaceLayer("Star UI", () =>
            {
                Draw(Main.spriteBatch, Main.LocalPlayer);
                return true;
            }, InterfaceScaleType.None));
        }
    }

}