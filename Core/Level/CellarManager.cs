using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Points;
using VocabValley.Core.Setting;

namespace VocabValley.Core.Level
{
    internal class CellarManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private PointsManager pointsManager;
        private SettingManager settingManager;

        public CellarManager(IModHelper helper, IMonitor monitor, 
            PointsManager pointsManager, SettingManager settingManager)
        {
            Helper = helper;
            Monitor = monitor;

            this.pointsManager = pointsManager;
            this.settingManager = settingManager;
        }

        public void onWarpCellar()
        {
            if (!settingManager.settingState.isCellarLocked)
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
            if (settingManager.settingState.isCellarLocked)
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
                    settingManager.settingState.isCellarLocked = false;
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
