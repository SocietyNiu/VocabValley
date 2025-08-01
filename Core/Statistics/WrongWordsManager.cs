using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.UI;

namespace VocabValley.Core.Statistics
{
    internal class WrongWordsManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private WrongWordPage wrongWordPage;
        private WordsManager wordsManager;
        
        int totalPage = 1;
        int currentPage = 1;
        List<Word> wrongWords;

        int pageCount = 8;

        public WrongWordsManager(IModHelper helper, IMonitor monitor, WordsManager wordsManager) 
        {
            wrongWordPage = new WrongWordPage(helper, monitor);
            this.wordsManager = wordsManager;
            wrongWords = wordsManager.getWrongWords();
            totalPage = wrongWords.Count() / pageCount + 1;

            wrongWordPage.currentPage = 1;
            wrongWordPage.totalPage = totalPage;

            setPageData();
            wrongWordPage.LastPage += () => LastPage();
            wrongWordPage.NextPage += () => NextPage();

            Game1.activeClickableMenu = wrongWordPage;
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
