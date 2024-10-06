using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

public class SorceryFightUI : UIState
{
    public CursedEnergyBar ceBar;

    public override void OnInitialize()
    {
        LoadCEBar();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
        ceBar.fillPercentage = player.cursedEnergy / player.maxCursedEnergy;
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
}