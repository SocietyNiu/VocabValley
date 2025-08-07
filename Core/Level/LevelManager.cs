using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Reward;
using VocabValley.Core.Saving;
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
        private FinalBossLevel finalBossLevel;

        public FinalProgress finalProgress;

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
            this.finalProgress = new FinalProgress();
            this.finalBossLevel = new FinalBossLevel(helper, monitor, wordsManager, this, finalProgress);
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

            if (finalProgress.IsEnd)
            {
                // 如果词库已经学习，则不允许进入
                Game1.drawObjectDialogue("该词库已学习完成，请换一本吧。");
                return;
            }

            // 如果没有未学习单词，无条件进入FinalBoss页
            if(wordsManager.getUnlearnedWordsCount() <= 0 || finalProgress.IsEnterFinal)
            {
                finalProgress.IsEnterFinal = true;
                onCallFinalBossLevel();
                return;
            }
            // 如果未复习单词积累到一定数量，则进入Boss层
            if (wordsManager.getUnreviewedWords().Count() >= thresholdCount)
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
        public void onCallFinalBossLevel()
        {
            Game1.warpFarmer("BabelTowerFinalBossLevel", 7, 14, false);
        }
        public void onCallFinalRewardLevel()
        {
            rewardManager.reset();
            rewardManager.setGrandRewardCard();
            Game1.warpFarmer("BabelTowerTop", 20, 29, false);
        }

        public void updateFinalProgress()
        {
            finalBossLevel.finalProgress = this.finalProgress;
        }
    }
}
