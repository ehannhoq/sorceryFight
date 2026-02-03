using System;
using System.Collections.Generic;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace sorceryFight.Content.Shops
{
    public class SorceryFightShopRegistrar : ModSystem
    {
        public static Dictionary<string, SorceryFightShop> LoadedShops { get; private set; }


        public override void PostSetupContent()
        {
            LoadedShops = new();
            foreach (var type in AssemblyManager.GetLoadableTypes(ModContent.GetInstance<SorceryFight>().Code))
            {
                if (type.IsAbstract || !typeof(SorceryFightShop).IsAssignableFrom(type))
                    continue;

                if (Activator.CreateInstance(type) is SorceryFightShop shop)
                {
                    shop.Initialize();
                    LoadedShops.Add(shop.ShopName, shop);
                }
            }
        }


        public static SorceryFightShop GetShop(string shopName)
        {
            if (LoadedShops.TryGetValue(shopName, out SorceryFightShop shop))
            {
                return shop;
            }
            throw new Exception($"No shop with name {shopName} is loaded!");
        }

        public override void Unload()
        {
            LoadedShops = null;
        }
    }
}