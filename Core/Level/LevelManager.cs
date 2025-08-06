using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Reward;
using VocabValley.Core.Setting;
using VocabValley.Core.Statistics;
using xTile.Tiles;

namespace VocabValley.Core.Level
{
    internal class LevelManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private WordsManager wordsManager;
        private RewardManager rewardManager;
        private SettingManager settingManager;
        // 学习多少单词进入复习环节
        private readonly int thresholdCount = 0;

        public LevelManager(IModHelper helper, IMonitor monitor, 
            WordsManager wordsManager, int thresholdCount,
            RewardManager rewardManager, SettingManager settingManager)
        {
            Helper = helper;
            Monitor = monitor;
            this.wordsManager = wordsManager;
            this.thresholdCount = thresholdCount;
            this.rewardManager = rewardManager;
            this.settingManager = settingManager;
        }

        public void onCallNextLevel()
        {
            if(settingManager.settingState.dailyLimitation && settingManager.DailyLimit <= 0)
            {
                Game1.drawObjectDialogue("你已达到爬塔限制，请明天再来吧。");
                return;
            }
            settingManager.DailyLimit--;
            // 对下一层的判断
            if(wordsManager.getUnreviewedWords().Count() >= thresholdCount)
            {
                onCallBossLevel();
            }
            else
            {
                onCallNormalLevel();
            }

        }
        public void onCallBossLevel()
        {
            Game1.warpFarmer("BabelTowerBossLevel", 7, 14, false);
        }

        public void onCallNormalLevel()
        {
            Game1.warpFarmer("BabelTowerNormalLevel", 7, 14, false);
        }
        public void onCallStairLevel()
        {
            var tile = Game1.player.TilePoint;
            Game1.warpFarmer("BabelTowerStairLevel", tile.X, tile.Y, false);
        }
        public void onCallRewardLevel()
        {
            rewardManager.reset();
            Game1.warpFarmer("BabelTowerRewardLevel", 7, 11, false);
        }

    }
}
