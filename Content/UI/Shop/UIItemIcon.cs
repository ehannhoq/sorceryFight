using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

public class UIItemIcon : UIElement
{
    private Item item;
    private float scale;

    public UIItemIcon(Item item, float scale = 1f)
    {
        this.item = item.Clone();
        this.item.noUseGraphic = false;
        this.item.stack = 1;
        this.scale = scale;

        OverflowHidden = false;
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
    }

    public void DrawIcon(SpriteBatch spriteBatch)
    {
        CalculatedStyle dims = GetDimensions();

        Main.instance.LoadItem(item.type);

        Vector2 position = new Vector2(
            dims.X + dims.Width * 0.5f,
            dims.Y + dims.Height * 0.5f
        );

        ItemSlot.DrawItemIcon(
            item,
            ItemSlot.Context.ShopItem,
            spriteBatch,
            position,
            scale,
            104f,
            Color.White
        );
    }
}