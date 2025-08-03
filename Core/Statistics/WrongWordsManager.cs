using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Points;
using VocabValley.Core.Setting;
using VocabValley.UI;

namespace VocabValley.Core.Statistics
{
    internal class WrongWordsManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private WrongWordPage wrongWordPage;
        
        private PointsManager pointsManager;
        private SettingManager settingManager;

        int totalPage = 1;
        int currentPage = 1;

        List<Word> wrongWords;

        int pageCount = 8;

        public WrongWordsManager(IModHelper helper, IMonitor monitor, WordsManager wordsManager, PointsManager pointsManager,
            SettingManager settingManager) 
        {
            wrongWordPage = new WrongWordPage(helper, monitor);
            this.pointsManager = pointsManager;
            this.settingManager = settingManager;
            wrongWords = wordsManager.getWrongWords();
            totalPage = wrongWords.Count() / pageCount + 1;

            wrongWordPage.currentPage = 1;
            wrongWordPage.totalPage = totalPage;

            setPageData();
            wrongWordPage.LastPage += () => LastPage();
            wrongWordPage.NextPage += () => NextPage();

            checkLockState();
        }
        
        private void checkLockState()
        {
            if (!settingManager.settingState.isWrongWordsPageLocked)
            {
                show();
                return;
            }

            if (pointsManager.points < 10)
            {
                Game1.drawObjectDialogue("错题本需要10知识碎片解锁");
                return;
            }
            else
            {
                string Question = "你想用10知识碎片解锁错题本吗?";
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
                                unLock();
                                break;
                            case "NO":
                                break;
                        }
                    });
            }
        }

        public void show()
        {
            Game1.activeClickableMenu = wrongWordPage;
        }
        public void unLock()
        {
            if (settingManager.settingState.isWrongWordsPageLocked)
            {
                if (pointsManager.points < 10)
                {
                    Game1.drawObjectDialogue("错题本需要100知识碎片解锁");
                    return;
                }
                else
                {
                    pointsManager.changePoints(-10);
                    Game1.playSound("money");
                    settingManager.settingState.isWrongWordsPageLocked = false;
                    Game1.drawObjectDialogue("错题本已解锁");
                }
            }
            else
            {
                Game1.drawObjectDialogue("错题本已解锁");
            }
        }

        public List<Word> getWordsByPage(int page)
        {
            return wrongWords
                .Skip((page - 1) * pageCount)
                .Take(pageCount)
                .ToList();
        }

        public void setPageData()
        {
            wrongWordPage.currentPage = currentPage;
            wrongWordPage.setWords(getWordsByPage(currentPage));
        }

        public void LastPage()
        {
            if(this.currentPage > 1)
            {
                currentPage--;
                setPageData();
            } 
        }

        public void NextPage()
        {
            if (this.currentPage < this.totalPage)
            {
                currentPage++;
                setPageData();
            }
        }
    }
}
