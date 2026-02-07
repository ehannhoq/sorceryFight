using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Shops;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.Shop
{
    public class SorceryFightShopUI : UIState
    {
        private readonly SorceryFightShop shop;

        private readonly UIImage itemList;
        private readonly UIImage itemShowcase;
        private readonly Texture2D itemContainerTexture;
        private readonly bool selectedItem;
        private readonly UIList itemListContent;
        private readonly UIScrollbar itemListScrollbar;

        private UIItemIcon showcaseItemIcon;
        private UIItemTooltipPanel showcaseItemTooltip;

        private SFButton buyButton;

        private readonly string uiPath;

        public SorceryFightShopUI(SorceryFightShop shop)
        {
            this.shop = shop;

            uiPath = ModContent.FileExists($"sorceryFight/Content/UI/Shop/{shop.ShopName}/ItemList") ? shop.ShopName : "DefaultUI";


            itemList = new UIImage(ModContent.Request<Texture2D>($"sorceryFight/Content/UI/Shop/{uiPath}/ItemList", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            itemShowcase = new UIImage(ModContent.Request<Texture2D>($"sorceryFight/Content/UI/Shop/{uiPath}/ItemShowcase", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            itemContainerTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/Shop/{uiPath}/ItemContainer", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;


            selectedItem = false;

            Width.Set(1024f, 0f);
            Height.Set(512f, 0f);

            Left.Set((Main.screenWidth / 2f) - (Width.Pixels / 2f), 0f);
            Top.Set((Main.screenHeight / 2f) - (Height.Pixels / 2f), 0f);

            itemShowcase.Left.Set(Width.Pixels - itemShowcase.Width.Pixels, 0f);

            Append(itemList);
            Append(itemShowcase);


            itemListContent = new UIList();
            itemListContent.Width.Set(itemList.Width.Pixels - 20f, 0f);
            itemListContent.Height.Set(itemList.Height.Pixels - 32f, 0f);
            itemListContent.Left.Set(0f, 0f);
            itemListContent.Top.Set(16f, 0f);
            itemListContent.ListPadding = 8f;
            itemListContent.PaddingLeft = (itemList.Width.Pixels - itemContainerTexture.Width) / 2f;
            itemList.Append(itemListContent);

            if (shop.Count > 9)
            {
                itemListScrollbar = new UIScrollbar();
                itemListScrollbar.SetView(100f, 1000f);
                itemListScrollbar.Height.Set(-1000f, 0f);
                itemListScrollbar.Left.Set(-1000f, 0f);
                itemListScrollbar.Top.Set(0f, 0f);
                itemListScrollbar.Width.Set(0f, 0f);
                itemList.Append(itemListScrollbar);
                itemListContent.SetScrollbar(itemListScrollbar);
            }

            InitializeItemContainers();

        }

        private void InitializeItemContainers()
        {
            for (int i = 0; i < shop.Count; i++)
            {
                Condition condition = shop.ShopItems[i].condition;

                if (condition != null)
                {
                    if (!condition.IsMet()) continue;
                }

                int index = i;

                SFButton itemContainer = new SFButton(itemContainerTexture, "");


                Item item = shop.GetItem(i);

                UIText itemName = new UIText(item.Name);
                itemName.HAlign = 0.5f;
                itemName.VAlign = 0.5f;
                itemName.TextColor = ItemRarity.GetColor(item.rare);
                itemContainer.Append(itemName);


                UIItemIcon itemIcon = new UIItemIcon(item, 1.5f);
                itemIcon.Width.Set(52f, 0f);
                itemIcon.Height.Set(52f, 0f);
                itemIcon.Left.Set(0f, 0f);
                itemIcon.Top.Set(-2.5f, 0f);
                itemContainer.Append(itemIcon);


                long price = shop.ShopItems[i].price;

                UIPriceElement priceElement = new UIPriceElement(price);
                priceElement.Width.Set(0f, 1f);
                priceElement.Height.Set(itemContainerTexture.Height, 0f);
                priceElement.VAlign = 0.5f;
                priceElement.PaddingRight = 24f;
                itemContainer.Append(priceElement);

                itemContainer.Width.Set(0f, 1f);
                itemContainer.Height.Set(itemContainerTexture.Height, 0f);
                itemListContent.Add(itemContainer);

                itemContainer.ClickAction += () => DisplaySelectedItem(index);
            }

            itemListContent.Recalculate();

            if (shop.Count > 9)
                itemListScrollbar.Recalculate();
        }

        private void DisplaySelectedItem(int index)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);

            Item item = shop.GetItem(index);

            showcaseItemIcon?.Remove();
            showcaseItemIcon = new UIItemIcon(item, 1.5f);

            showcaseItemTooltip?.Remove();
            showcaseItemTooltip = new UIItemTooltipPanel(item, textScale: 0.75f, lineSpacing: 2f);

            buyButton?.Remove();
            buyButton = new(ModContent.Request<Texture2D>($"sorceryFight/Content/UI/Shop/{uiPath}/BuyItem", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "");


            showcaseItemIcon.Width.Set(52f, 0f);
            showcaseItemIcon.Height.Set(52f, 0f);
            showcaseItemIcon.Left.Set(itemShowcase.Width.Pixels / 2f - (showcaseItemIcon.Width.Pixels / 2f), 0f);
            showcaseItemIcon.Top.Set(itemShowcase.Height.Pixels / 5f - (showcaseItemIcon.Height.Pixels / 2f), 0f);


            showcaseItemTooltip.Left.Set(10f, 0f);
            showcaseItemTooltip.Top.Set(2 * itemShowcase.Height.Pixels / 5f, 0f);

            buyButton.Left.Set((itemShowcase.Width.Pixels / 2f) - (buyButton.Width.Pixels / 2f), 0f);
            buyButton.Top.Set((6 * itemShowcase.Height.Pixels / 7f) - (buyButton.Height.Pixels / 2f), 0f);

            buyButton.ClickAction += () =>
            {
                if (!Main.LocalPlayer.CanAfford(shop.ShopItems[index].price))
                {
                    SoundEngine.PlaySound(SoundID.MenuClose);
                    return;
                }


                if (Main.mouseItem.type == item.type && !Main.mouseItem.IsAir)
                {
                    if (Main.mouseItem.stack < Main.mouseItem.maxStack)
                    {
                        Main.mouseItem.stack++;
                        SoundEngine.PlaySound(SoundID.Grab);
                        Main.LocalPlayer.BuyItem(shop.ShopItems[index].price);
                    }
                }
                else
                {
                    Item copy = item.Clone();
                    copy.stack = 1;
                    Main.mouseItem = copy;
                    SoundEngine.PlaySound(SoundID.Grab);
                    Main.LocalPlayer.BuyItem(shop.ShopItems[index].price);
                }
            };

            itemShowcase.Append(showcaseItemIcon);
            itemShowcase.Append(showcaseItemTooltip);
            itemShowcase.Append(buyButton);
            itemShowcase.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            if (ContainsPoint(Main.MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (!Main.playerInventory)
            {
                ModContent.GetInstance<SorceryFightUISystem>().ResetUI();
            }
        }

        private class SorceryFightShopScrollStopper : ModSystem
        {
            public override void PostUpdateInput()
            {
                if (ModContent.GetInstance<SorceryFightUISystem>().shopUIOpen)
                {
                    PlayerInput.ScrollWheelDelta = 0;
                }
            }
        }

        protected override void DrawChildren(SpriteBatch spriteBatch)
        {
            base.DrawChildren(spriteBatch);

            DrawItemIconsRecursive(this, spriteBatch);
        }

        private void DrawItemIconsRecursive(UIElement element, SpriteBatch spriteBatch)
        {
            if (element is UIItemIcon icon)
                icon.DrawIcon(spriteBatch);

            foreach (UIElement child in element.Children)
                DrawItemIconsRecursive(child, spriteBatch);
        }
    }
}