using System.Collections.Generic;
using Terraria;

namespace sorceryFight.Content.Shops
{
    public struct ShopItem
    {
        internal int item;
        internal int price;
        internal Condition condition;

        internal ShopItem(int item, int price, Condition condition)
        {
            this.item = item;
            this.price = price;
            this.condition = condition;
        }
    }

    public abstract class SorceryFightShop
    {
        /// <summary>
        /// Used to connect shop name json value in InteractableJson to the actual shop. Also used to draw custom shop UI, if exists.
        /// </summary>
        public string ShopName { get; internal set; }


        public List<ShopItem> ShopItems { get; private set; }


        /// <summary>
        /// Returns the numbner of items available in this shop.
        /// </summary>
        public int Count => ShopItems.Count;


        /// <summary>
        /// Used to add items to the shop.
        /// </summary>
        public abstract void Initialize();

        protected SorceryFightShop()
        {
            ShopItems = [];
        }

        /// <summary>
        /// Adds an item to this shop.
        /// </summary>
        public void AddItem(int item, int price, Condition condition = null)
        {
            ShopItem shopItem = new(item, price, condition);
            ShopItems.Add(shopItem);
        }

        public Item GetItem(int index)
        {
            Item item = new();
            item.SetDefaults(ShopItems[index].item);
            return item.Clone();
        }
    }
}