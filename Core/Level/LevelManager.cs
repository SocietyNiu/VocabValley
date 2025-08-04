using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Reward;
using xTile.Tiles;

namespace VocabValley.Core.Level
{
    internal class LevelManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private WordsManager wordsManager;
        private RewardManager rewardManager;

        // 学习多少单词进入复习环节
        private readonly int thresholdCount = 0;

        public LevelManager(IModHelper helper, IMonitor monitor, 
            WordsManager wordsManager, int thresholdCount,
            RewardManager rewardManager)
        {
            Helper = helper;
            Monitor = monitor;
            this.wordsManager = wordsManager;
            this.thresholdCount = thresholdCount;
            this.rewardManager = rewardManager;
        }

        public void onCallNextLevel()
        {
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
