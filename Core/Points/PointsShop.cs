using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.UI;

namespace VocabValley.Core.Points
{
    internal class PointsShop
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private Dictionary<ISalable, ItemStockInformation> stock;
        private PointsManager pointsManager;

        public PointsShop(IModHelper helper, IMonitor monitor, PointsManager pointsManager)
        {
            Helper = helper;
            Monitor = monitor;
            this.pointsManager = pointsManager;
        }

        public void setGoods()
        {
            stock = new Dictionary<ISalable, ItemStockInformation>
            {
                [new StardewValley.Object("388", 1)] = new(price: 10, stock: 20),        // 木头
                [new StardewValley.Object("390", 1)] = new(price: 15, stock: 20),        // 石头
                [new StardewValley.Object("472", 1)] = new(price: 50, stock: 30)         // 小麦种子
            };
        }

        public void showShop()
        {
            bool onPurchase(ISalable item, Farmer who, int countTaken, ItemStockInformation itemInfo)
            {
                int cost = itemInfo.Price * countTaken;
                if (pointsManager.points < cost)
                    return false;                         

                pointsManager.points -= cost;             
                return true;                              
            };

            var menu = new PointsPage(Helper, Monitor, "VocabValley.PointsShop", stock, onPurchase, pointsManager);
            Game1.activeClickableMenu = menu;

        }

    }
}
