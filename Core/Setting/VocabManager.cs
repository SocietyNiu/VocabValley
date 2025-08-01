using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VocabValley.UI;

namespace VocabValley.Core.Setting
{
    internal class VocabManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private string[] files;
        private readonly string filePath;

        public string vocabChosen = "";

        private SettingVocabPage settingVocabPage;
        public Action<string>? OnVocabChanged;
        public VocabManager(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
            filePath = Path.Combine(Helper.DirectoryPath, "vocabulary");

            
        }
        public void setVocab(string vocabChosen)
        {
            Game1.drawObjectDialogue("词库已更换为：" + vocabChosen);
            this.vocabChosen = vocabChosen;
            settingVocabPage.vocabChosen = vocabChosen;
        }
        public void scanPath()
        {
            this.files = Directory.GetFiles(filePath, "*.txt")
                .Select(path => Path.GetFileName(path))
                .ToArray();
        }
        public void init()
        {
            settingVocabPage = new SettingVocabPage(Helper, Monitor);
            settingVocabPage.SetVocab += (string fileName) =>
            {
                setVocab(fileName);
                OnVocabChanged?.Invoke(fileName);
            };

            this.scanPath();
            settingVocabPage.setData(this.files, this.vocabChosen);
        }
        public void show()
        {
            Game1.activeClickableMenu = settingVocabPage;

        }
    }
}
