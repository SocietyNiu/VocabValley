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

namespace VocabValley
{
    public class ModEntry : Mod
    {
        private WordsManager wordsManager = null;
        private LearningPageManager learningPageManager = null;
        private RewardManager rewardManager = null;
        private LevelManager levelManager = null;
        private SavingManager savingManager = null;
        private VocabManager vocabManager = null;
        private SettingManager settingManager = null;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            GameLocation.RegisterTileAction("onLearningPageCall", onLearningPageCall);
            GameLocation.RegisterTileAction("onRewardPageCall", onRewardPageCall);
            GameLocation.RegisterTileAction("onBossPageCall", onBossPageCall);
            GameLocation.RegisterTouchAction("onLevelPageCall", onLevelPageCall);
            GameLocation.RegisterTileAction("onWrongWordsPageCall", onWrongWordsPageCall);
            GameLocation.RegisterTileAction("onVocabPageCall", onVocabPageCall);

            // TODO: 把Vocab融合到Setting里
            vocabManager = new VocabManager(Helper, Monitor);
            wordsManager = new WordsManager(Helper, Monitor);
            savingManager = new SavingManager(Helper, Monitor, vocabManager, wordsManager);
            settingManager = new SettingManager(Helper, Monitor);
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            rewardManager = new RewardManager(Helper, Monitor, "normal");
            levelManager = new LevelManager(Helper, Monitor, wordsManager, 10);
        }
        private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
        {
            if(e.Button == SButton.L)
            {
                RewardManager rm = new RewardManager(Helper, Monitor, "grand");
                rm.onRewardPageCall();
            }
            if(e.Button == SButton.O)
            {
                settingManager.setPause(true);
            }
        }
        private bool onLearningPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            learningPageManager = new LearningPageManager(Helper, Monitor, wordsManager, 3, levelManager);
            learningPageManager.onLearningPageCall();
            return true;
        }

        private bool onRewardPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            rewardManager.onRewardPageCall();
            return true;
        }

        private bool onBossPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            BossPageManager bossPageManager = new BossPageManager(Helper, Monitor, wordsManager, 0, levelManager);
            bossPageManager.onBossPageCall();
            return true;
        }
        private bool onVocabPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            vocabManager.init();
            vocabManager.show();
            return true;
        }
        private bool onWrongWordsPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            WrongWordsManager wrongWordsManager = new WrongWordsManager(Helper, Monitor, wordsManager);
            return true;
        }
        private void onLevelPageCall(GameLocation location, string[] args, Farmer player, Vector2 tile)
        {
            /*
            BossPageManager bossPageManager = new BossPageManager(Helper, Monitor, wordsManager, 50, 180);
            bossPageManager.onBossPageCall();
            return true;
            */
            levelManager.onCallNextLevel();
        }
    }
}
