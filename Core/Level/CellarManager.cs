using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Points;

namespace VocabValley.Core.Level
{
    internal class CellarManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private bool isLocked = true;

        private PointsManager pointsManager;

        public CellarManager(IModHelper helper, IMonitor monitor, PointsManager pointsManager)
        {
            Helper = helper;
            Monitor = monitor;

            this.pointsManager = pointsManager;
        }

        public void onWarpCellar()
        {
            if (!isLocked)
            {
                Game1.warpFarmer("BabelTowerCellar", 7, 14, false);
                return;
            }
            if(pointsManager.points < 100)
            {
                Game1.drawObjectDialogue("地下室需要100知识碎片解锁");
                return;
            }
            else
            {
                string Question = "你想用100知识碎片解锁地下室吗?";
                Response[] opts = {
                new("YES", "是的"),
                new("NO",  "我再想想")
            };
                Game1.currentLocation.createQuestionDialogue(
                    Question,
                    opts,
                    (farmer, ans) =>
                    {
                        switch (ans)
                        {
                            case "YES":
                                unLockCellar();
                                break;
                            case "NO":
                                break;
                        }
                    });
            }
        }

        public void unLockCellar()
        {
            if (isLocked)
            {
                if (pointsManager.points < 100)
                {
                    Game1.drawObjectDialogue("地下室需要100知识碎片解锁");
                    return;
                }
                else
                {
                    pointsManager.changePoints(-100);
                    Game1.playSound("money");
                    isLocked = false;
                    Game1.drawObjectDialogue("地下室已解锁");
                }
            }
            else
            {
                Game1.drawObjectDialogue("地下室已解锁");
            }
        }

    }
}
