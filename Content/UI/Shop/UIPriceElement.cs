using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI;

namespace sorceryFight.Content.UI.Shop
{
    public class UIPriceElement : UIElement
    {
        private int numPlatinumCoins;
        private int numGoldCoins;
        private int numSilverCoins;
        private int numCopperCoins;

        private const float Gap = 8f;

        public UIPriceElement(long price)
        {
            ConvertPrice(price);

            Width.Set(0f, 1f);
            Height.Set(32f, 0f);

            float currentX = 0f;

            AddCoin(ref currentX, ItemID.CopperCoin, numCopperCoins);
            AddCoin(ref currentX, ItemID.SilverCoin, numSilverCoins);
            AddCoin(ref currentX, ItemID.GoldCoin, numGoldCoins);
            AddCoin(ref currentX, ItemID.PlatinumCoin, numPlatinumCoins);
        }

        private void AddCoin(ref float currentX, int itemID, int amount)
        {
            if (amount <= 0)
                return;

            Main.instance.LoadItem(itemID);
            Texture2D texture = TextureAssets.Item[itemID].Value;

            UIImage coinImage = new UIImage(texture);
            UIText coinText = new UIText(amount.ToString());

            coinText.Recalculate();
            coinImage.Recalculate();

            coinImage.VAlign = 0.5f;
            coinText.VAlign = 0.5f;

            currentX += coinText.Width.Pixels;
            coinText.Left.Set(-currentX, 1f);

            currentX += Gap;
            currentX += coinImage.Width.Pixels;
            coinImage.Left.Set(-currentX, 1f);

            currentX += Gap * 2.5f;

            Append(coinImage);
            Append(coinText);
        }

        private void ConvertPrice(long price)
        {
            numCopperCoins = (int)(price % 100);
            price /= 100;

            numSilverCoins = (int)(price % 100);
            price /= 100;

            numGoldCoins = (int)(price % 100);
            price /= 100;

            numPlatinumCoins = (int)price;
        }
    }
}