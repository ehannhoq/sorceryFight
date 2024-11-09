using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using sorceryFight.Content.UI.CursedTechniqueMenu;
using sorceryFight.Content.UI.InnateTechniqueSelector;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

public class SorceryFightUI : UIState
{
    public CursedEnergyBar ceBar;
    public CursedTechniqueMenu ctMenu;

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();

        if (player.innateTechnique.Name == "No Innate Technique")
        {

            if (player.yourPotentialSwitch)
                {
                    InnateTechniqueSelector innateTechniqueSelector = new InnateTechniqueSelector();
                    Append(innateTechniqueSelector);
                    Recalculate();
                    player.yourPotentialSwitch = false;
                }

            return;
        }

        if (player.yourPotentialSwitch)
        {}

        if (ceBar == null)
            LoadCEBar();

        ceBar.fillPercentage = player.cursedEnergy / player.maxCursedEnergy;

        if (SFKeybinds.OpenTechniqueUI.JustPressed)
        {
            {
                Elements.Clear();
                InnateTechniqueSelector innateTechniqueSelector = new InnateTechniqueSelector();
                Append(innateTechniqueSelector);
            }

            if (!Elements.Contains(ctMenu))
            {
                ctMenu = new CursedTechniqueMenu(player);
                player.hasUIOpen = true;
                Append(ctMenu);
            }
            else
            {
                Elements.Remove(ctMenu);
                player.hasUIOpen = false;
            }
        }
    }

    public void LoadCEBar()
    {
        string borderBarPath = "sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarBorder";
        string fillBarPath = "sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarFill";

        ModContent.Request<Texture2D>(borderBarPath).Wait();
        ModContent.Request<Texture2D>(fillBarPath).Wait();

        var borderTexture = ModContent.Request<Texture2D>(borderBarPath).Value;
        var barTexture = ModContent.Request<Texture2D>(fillBarPath).Value;

        UIImage borderBar = new UIImage(borderTexture);
        ceBar = new CursedEnergyBar(barTexture);

        int x = 200;
        int y = 200;

        borderBar.Left.Set(x, 0f);
        borderBar.Top.Set(y, 0f);
        ceBar.Left.Set(x + ((borderTexture.Width - barTexture.Width) / 2), 0f);
        ceBar.Top.Set(y, 0f);

        Append(borderBar);
        Append(ceBar);
    }

    public static bool MouseHovering(UIElement ui, Texture2D texture)
    {
        Vector2 mousePos = Main.MouseScreen;
        CalculatedStyle dimensions = ui.GetDimensions();
        
        return mousePos.X >= dimensions.X && mousePos.X <= dimensions.X + texture.Width && 
                mousePos.Y >= dimensions.Y && mousePos.Y <= dimensions.Y + texture.Height;
    }
}