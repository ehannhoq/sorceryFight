using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;

public class BloodEnergyBar : UIElement
{
    SorceryFightPlayer sfPlayer;
    public UIImage border;
    public ValueBar beBarValue;
    bool isDragging;
    private bool initialized;
    bool hasRightClicked;
    Vector2 offset;
    Texture2D borderTexture;


    public BloodEnergyBar(Texture2D borderTexture, Texture2D barTexture)
    {
        if (Main.dedServ) return;

        IgnoresMouseInteraction = true;



        this.borderTexture = borderTexture;

        Width.Set(borderTexture.Width, 0f);
        Height.Set(borderTexture.Height, 0f);

        border = new UIImage(borderTexture);
        ModContent.GetInstance<SorceryFight>().Logger.Debug("BORDER TEXTURE WIDTH:" + border.Width);
        Append(border);

        beBarValue = new ValueBar(barTexture, Orientation.Vertical);
        beBarValue.Left.Set((borderTexture.Width - barTexture.Width) / 2f, 0f);
        beBarValue.Top.Set(0, 0f);
        Append(beBarValue);

        Left.Set(1200, 0f);
        Top.Set(20, 0f);

        border.IgnoresMouseInteraction = true;
        beBarValue.IgnoresMouseInteraction = true;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);



        if (IsMouseHovering)
        {
            var player = Main.LocalPlayer.SorceryFight();
            Main.hoverItemName = $"{SFUtils.GetLocalizationValue($"Mods.sorceryFight.UI.BloodEnergyBar.BE")} {Math.Round((decimal)player.bloodEnergy, 0)} / {player.maxBloodEnergy}\n"
                                + $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.BloodEnergyBar.RegenRate")} {player.bloodEnergyRegenPerSecond} BE/s\n"
                                + SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.BloodEnergyBar.ToolTip");
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        sfPlayer = Main.LocalPlayer.SorceryFight();

        if (!initialized)
        {
            initialized = true;
            SetPosition();

        }


        if (Main.playerInventory && SorceryFightUI.MouseHovering(this, beBarValue.barTexture) && Main.mouseLeft && !isDragging)
        {
            isDragging = true;
            offset = new Vector2(Main.mouseX, Main.mouseY) - new Vector2(Left.Pixels, Top.Pixels);
        }

        if (isDragging)
        {
            float clampedLeft = Math.Clamp(Main.mouseX - offset.X, 0f, Main.screenWidth - borderTexture.Width);
            float clampedTop = Math.Clamp(Main.mouseY - offset.Y, 0f, Main.screenHeight - borderTexture.Height);

            Left.Set(clampedLeft, 0f);
            Top.Set(clampedTop, 0f);
            Recalculate();

            if (!Main.mouseLeft)
            {
                isDragging = false;
                Recalculate();
            }
        }

        if (Main.playerInventory && SorceryFightUI.MouseHovering(this, beBarValue.barTexture) && Main.mouseRight && !isDragging)
        {
            Rectangle mouseRect = new Rectangle((int)Main.MouseWorld.X - 8, (int)Main.MouseWorld.Y - 8, 16, 16);
            if (!hasRightClicked)
            {
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                {
                    sfPlayer.BEBarPos = Vector2.Zero;
                    SetPosition();
                    CombatText.NewText(mouseRect, Color.White, "UI Position Reset!");
                    Main.mouseRightRelease = true;
                }
                else
                {
                    sfPlayer.BEBarPos = new Vector2(this.Left.Pixels, this.Top.Pixels);
                    CombatText.NewText(mouseRect, Color.White, "UI Position Saved!");
                    Main.mouseRightRelease = true;
                }
            }

        }


        if (Main.mouseRight && SorceryFightUI.MouseHovering(this, beBarValue.barTexture))
        {
            hasRightClicked = true;
        }
        else if (Main.mouseRightRelease && SorceryFightUI.MouseHovering(this, beBarValue.barTexture))
        {
            hasRightClicked = false;
        }


    }

    void SetPosition()
    {
        sfPlayer = Main.LocalPlayer.SorceryFight();
        if (sfPlayer.BEBarPos == Vector2.Zero)
        {
            Left.Set(1300, 0f);
            Top.Set(20, 0f);
        }
        else
        {
            Left.Set(sfPlayer.BEBarPos.X, 0f);
            Top.Set(sfPlayer.BEBarPos.Y, 0f);
        }
    }
}