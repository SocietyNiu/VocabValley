using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Points;

namespace VocabValley.UI
{
    internal class PointsPage: ShopMenu
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private Texture2D pointsTexture;
        private Rectangle source;
        private PointsManager _pm;

        public PointsPage(IModHelper helper, IMonitor monitor,
            string shopId,
            Dictionary<ISalable, ItemStockInformation> stock,
            OnPurchaseDelegate onPurchase,
            PointsManager pm
            ):
            base(
                shopId: shopId,
                itemPriceAndStock: stock,
                currency: -5,
                on_purchase: onPurchase,
                on_sell: null,
                playOpenSound: true,
                who: null
                )
        {
            Helper = helper;
            Monitor = monitor;
            pointsTexture = helper.GameContent.Load<Texture2D>("LooseSprites/VocabValley_Points");
            source = new Rectangle(32, 0, 16, 16);

            _pm = pm;

            canPurchaseCheck = (slotIdx) =>
            {
                var info = itemPriceAndStock[forSale[slotIdx]];
                return _pm.points >= info.Price;
            };

        }
        
        private string getHoveredItemExtraItemIndex()
        {
            if (hoveredItem != null && itemPriceAndStock != null && itemPriceAndStock.TryGetValue(hoveredItem, out var value) && value.TradeItem != null)
            {
                return value.TradeItem;
            }

            return null;
        }
        private int getHoveredItemExtraItemAmount()
        {
            if (hoveredItem != null && itemPriceAndStock != null && itemPriceAndStock.TryGetValue(hoveredItem, out var value) && value.TradeItem != null && value.TradeItemCount.HasValue)
            {
                return value.TradeItemCount.Value;
            }

            return 5;
        }

        public override void draw(SpriteBatch b)
        {
            // 先画基类
            base.draw(b);

            // 画知识碎片，盖住之前的货币
            for (int i = 0; i < forSaleButtons.Count; i++)
            {
                ClickableComponent clickableComponent = forSaleButtons[i];
                if (currentItemIndex + i >= forSale.Count)
                {
                    continue;
                }

                bool flag = canPurchaseCheck != null && !canPurchaseCheck(currentItemIndex + i);
                ISalable salable = forSale[currentItemIndex + i];
                ItemStockInformation itemStockInformation = itemPriceAndStock[salable];
                StackDrawType stackDrawType = GetStackDrawType(itemStockInformation, salable);
                string text = salable.DisplayName;
                if (itemStockInformation.Price > 0)
                {
                    Utility.drawWithShadow(b, pointsTexture, 
                        new Vector2(clickableComponent.bounds.Right - 70, clickableComponent.bounds.Y + 25),
                        source, Color.White * ((!flag) ? 1f : 0.25f), 0f, 
                        Vector2.Zero, 4f, flipped: false, -1f, -1, -1, (!flag) ? 0.35f : 0f);
                }
                if (hoverText != "")
                {
                    Item item = hoveredItem as Item;
                    ISalable salable2 = hoveredItem;
                    if (salable2 != null && salable2.IsRecipe)
                    {
                        IClickableMenu.drawToolTip(b, " ", boldTitleText, item, heldItem != null, -1, currency, getHoveredItemExtraItemIndex(), getHoveredItemExtraItemAmount(), new CraftingRecipe(item?.BaseName ?? hoveredItem.Name), (hoverPrice > 0) ? hoverPrice : (-1));
                    }
                    else
                    {
                        IClickableMenu.drawToolTip(b, hoverText, boldTitleText, item, heldItem != null, -1, currency, getHoveredItemExtraItemIndex(), getHoveredItemExtraItemAmount(), null, (hoverPrice > 0) ? hoverPrice : (-1));
                    }
                }

                drawMouse(b);
            }
        }

    }
}
