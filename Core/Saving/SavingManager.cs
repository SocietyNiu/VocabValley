using StardewModdingAPI;
using StardewModdingAPI.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Points;
using VocabValley.Core.Setting;

namespace VocabValley.Core.Saving
{
    internal class SavingManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private List<Word> words;
        public WordsManager wordsManager;
        public VocabManager vocabManager;
        public PointsManager pointsManager;

        private Dictionary<int, SaveState> _progress = null!;
        private Config _config;
        private Dictionary<string, Dictionary<int, SaveState>> _allProgress;
        private OtherState _otherState;


        public SavingManager(IModHelper helper, IMonitor monitor,
            VocabManager vocabManager, WordsManager wordsManager,
            PointsManager pointsManager)
        {
            Helper = helper;
            Monitor = monitor;

            Helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            Helper.Events.GameLoop.Saving += this.onSaving;

            this.wordsManager = wordsManager;
            this.vocabManager = vocabManager;
            this.pointsManager = pointsManager;
            this.vocabManager.OnVocabChanged += (string fileName) =>
            {
                updateProgress(fileName);
            };
        }


        public void onSaveLoaded(object? s, SaveLoadedEventArgs e)
        {
            // 读取设置
            // 如果设置中没有词库选择，则先默认四级词库
            _config = Helper.Data.ReadSaveData<Config>("config") ?? new Config();
            if (string.IsNullOrEmpty(_config.fileName))
                _config.fileName = "CET4.txt";
            vocabManager.vocabChosen = _config.fileName;

            // 把总进度存入cache
            var allProgress = Helper.Data.ReadSaveData<Dictionary<string, Dictionary<int, SaveState>>>("allProgress")
                    ?? new();
            _allProgress = allProgress;

            // 从cache中抽出对应词库的记录, 没有则新建
            if (!_allProgress.TryGetValue(_config.fileName, out _progress))
                _progress = new Dictionary<int, SaveState>();

            // 从单词管理器中加载对应的词库
            // 并取对应的单词列表引用
            wordsManager.getWordsFromFile(_config.fileName);
            wordsManager.initialization();
            words = wordsManager.wordsList;

            // 针对每个单词，载入对应的记录
            foreach (var word in words)
            {
                // 先尝试在存档中读取数据，否则新建记录
                if (_progress.TryGetValue(word.ID, out var state))
                {
                    word.isLearned = state.IsLearned;
                    word.reviewCount = state.ReviewCount;
                    word.wrongCount = state.WrongCount;
                }
                else
                {
                    _progress[word.ID] = new SaveState();
                }
            }

            // TODO: 清理词库已删除的词 

            _otherState = Helper.Data.ReadSaveData<OtherState>("otherState") ?? new OtherState();
            pointsManager.points = _otherState.Points;
        }

        public void onSaving(object? s, SavingEventArgs e)
        {
            foreach(var word in words)
            {
                _progress[word.ID] = new SaveState
                {
                    IsLearned = word.isLearned,
                    ReviewCount = word.reviewCount,
                    WrongCount = word.wrongCount
                };
            }

            _config.fileName = vocabManager.vocabChosen;
            Helper.Data.WriteSaveData("config", _config);

            _allProgress[_config.fileName] = _progress;
            Helper.Data.WriteSaveData("allProgress", _allProgress);

            _otherState.Points = pointsManager.points;
            Helper.Data.WriteSaveData("otherState", _otherState);

        }
        public void updateProgress(string fileName)
        {
            flushWords2Progress();
            // 先把现在的词库数据存入Cache
            _allProgress[_config.fileName] = _progress;

            // 从cache中抽出对应词库的记录, 没有则新建
            if (!_allProgress.TryGetValue(fileName, out _progress))
                _progress = new Dictionary<int, SaveState>();

            // 从单词管理器中加载对应的词库
            // 并取对应的单词列表引用
            wordsManager.getWordsFromFile(fileName);
            wordsManager.initialization();
            words = wordsManager.wordsList;

            // 针对每个单词，载入对应的记录
            foreach (var word in words)
            {
                // 先尝试在存档中读取数据，否则新建记录
                if (_progress.TryGetValue(word.ID, out var state))
                {
                    word.isLearned = state.IsLearned;
                    word.reviewCount = state.ReviewCount;
                    word.wrongCount = state.WrongCount;
                }
                else
                {
                    _progress[word.ID] = new SaveState();
                }
            }

            _config.fileName = fileName;

        }

        private void flushWords2Progress()
        {
            if (words == null || _progress == null) return;

            foreach (var w in words)
                _progress[w.ID] = new SaveState
                {
                    IsLearned = w.isLearned,
                    ReviewCount = w.reviewCount,
                    WrongCount = w.wrongCount
                };
        }
    }
}
