using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using xTile.Tiles;
using SObject = StardewValley.Object;
using StardewValley.GameData.Objects;
using VocabValley.UI;
using VocabValley.Utils;
using Microsoft.Xna.Framework;
using VocabValley.Core.Statistics;
using VocabValley.Core.Setting;
using VocabValley.Core.Saving;
using VocabValley.Core.Reward;
using VocabValley.Core.Points;
using VocabValley.Core.Model;
using VocabValley.Core.Level;


namespace VocabValley.Core
{
    internal class CoreManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        // 单词管理器
        private WordsManager wordsManager = null;

        // 层级管理器
        private LevelManager levelManager = null;

        // 存档管理器
        private SavingManager savingManager = null;

        // 词库管理器
        private VocabManager vocabManager = null;

        // 设置管理器
        private SettingManager settingManager = null;

        // 地下室管理器
        private CellarManager cellarManager = null;

        // 积分管理器
        private PointsManager pointsManager = null;

        // 奖励管理器
        private RewardManager rewardManager = null;

        // 统计数据管理器
        private StatisticsManager statisticsManager = null;

        public CoreManager(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;

            helper.Events.GameLoop.SaveLoaded += onSaveLoaded;
            helper.Events.GameLoop.DayStarted += onDayStarted;

            GameLocation.RegisterTileAction("onLearningPageCall", onLearningPageCall);
            GameLocation.RegisterTileAction("onRewardPageCall", onRewardPageCall);
            GameLocation.RegisterTileAction("onBossPageCall", onBossPageCall);
            GameLocation.RegisterTileAction("onWrongWordsPageCall", onWrongWordsPageCall);
            GameLocation.RegisterTileAction("onVocabPageCall", onVocabPageCall);
            GameLocation.RegisterTileAction("onSettingPageCall", onSettingPageCall);
            GameLocation.RegisterTileAction("onStatisticsPageCall", onStatisticsPageCall);
            GameLocation.RegisterTileAction("onRewardPremium", onRewardPremium);
            GameLocation.RegisterTouchAction("onLevelPageCall", onLevelPageCall);
            GameLocation.RegisterTouchAction("onWarpCellar", onWarpCellar);
            
            vocabManager = new VocabManager(Helper, Monitor);
            wordsManager = new WordsManager(Helper, Monitor);
            pointsManager = new PointsManager(Helper, Monitor);
            settingManager = new SettingManager(Helper, Monitor);
            rewardManager = new RewardManager(Helper, Monitor, pointsManager);
            levelManager = new LevelManager(Helper, Monitor, wordsManager, 30, rewardManager, settingManager);
            statisticsManager = new StatisticsManager(Helper, Monitor, wordsManager);
            savingManager = new SavingManager(Helper, Monitor, vocabManager, wordsManager, pointsManager, settingManager, statisticsManager, rewardManager, levelManager);
            cellarManager = new CellarManager(Helper, Monitor, pointsManager, settingManager);
            
        }

        public void onSaveLoaded(object? s, SaveLoadedEventArgs e)
        {
            // 有些材质导入不能再Entry时进行
            // 放在读取存档时进行
            rewardManager.init();
        }
        private bool onLearningPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            // 注册普通层学习事件
            LearningPageManager learningPageManager 
                    = new LearningPageManager(Helper, Monitor, wordsManager, 10, levelManager, pointsManager, statisticsManager);
            learningPageManager.onLearningPageCall();
            return true;
        }

        private bool onRewardPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            // 注册奖励页面事件
            rewardManager.onRewardPageCall();
            return true;
        }

        private bool onBossPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            // 注册Boss页面事件
            BossPageManager bossPageManager 
                = new BossPageManager(Helper, Monitor, wordsManager, 0, levelManager, statisticsManager);
            bossPageManager.onBossPageCall();
            return true;
        }
        private bool onVocabPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            // 注册词库更新页面事件
            vocabManager.init();
            vocabManager.show();
            return true;
        }
        private bool onWrongWordsPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            // 注册错题页事件
            WrongWordsManager wrongWordsManager = new WrongWordsManager(Helper, Monitor, wordsManager, pointsManager, settingManager);
            return true;
        }
        
        private bool onSettingPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            settingManager.onPageCall();
            return true;
        }

        private bool onStatisticsPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            statisticsManager.onShowPageCall();
            return true;
        }

        private bool onRewardPremium(GameLocation location, string[] args, Farmer player, Point point)
        {
            rewardManager.updatePremiumLevel();
            return true;
        }

        private void onLevelPageCall(GameLocation location, string[] args, Farmer player, Vector2 tile)
        {
            // 注册塔内层级页面事件
            levelManager.onCallNextLevel();
        }

        private void onWarpCellar(GameLocation location, string[] args, Farmer player, Vector2 tile)
        {
            cellarManager.onWarpCellar();
        }

        private void onDayStarted(object? sender, DayStartedEventArgs e)
        {
            if (settingManager.settingState.dailyLimitation)
            {
                settingManager.DailyLimit = 5;
            }
        }
    }
}
