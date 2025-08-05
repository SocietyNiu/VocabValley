using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.UI;
using VocabValley.Utils;
using VocabValley.Core.Points;
using VocabValley.Core.Model;
using VocabValley.Core.Statistics;

namespace VocabValley.Core.Level
{
    internal class LearningPageManager
    {

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private readonly WordsManager wordsManager;
        private readonly LevelManager levelManager;
        private readonly PointsManager pointsManager;
        private readonly StatisticsManager statisticsManager;
        // 单次学习单词数量
        public int learningWordsCount { get; set; }

        // 为方便答错单词重新学习，维护一个队列
        private Queue<Word> wordQuizzesQueue;

        public LearningPageManager(IModHelper helper, IMonitor monitor, 
            WordsManager WordsManager, int LearningWordsCount, 
            LevelManager levelManager, PointsManager pointsManager,
            StatisticsManager statisticsManager)
        {
            Helper = helper;
            Monitor = monitor;

            wordsManager = WordsManager;
            learningWordsCount = LearningWordsCount;
            this.pointsManager = pointsManager;
            this.levelManager = levelManager;
            this.statisticsManager = statisticsManager;
        }

        // 准备所学单词

        public List<Word> initialization()
        {
            try
            {
                return wordsManager.getUnlearnedWordsInOrder(learningWordsCount);
            }
            catch (Exception ex)
            {
                Monitor.Log(ex.Message, LogLevel.Error);
                return new List<Word>();
            }
        }

        // 正式调用
        public void onLearningPageCall()
        {
            // 获取所学单词
            var quizList = initialization();
            if(quizList.Count <= 0)
            {
                Game1.drawObjectDialogue("没有需要新学的单词！");
                return;
            }
            
            // 压入所有的quiz
            wordQuizzesQueue = new Queue<Word>();
            foreach(Word wordQuiz in quizList)
            {
                wordQuizzesQueue.Enqueue(wordQuiz);
            }

            WordLearningPage wordPage = new WordLearningPage(Helper, Monitor, wordsManager);

            wordPage.setQuizCount(wordQuizzesQueue.Count());

            // 展示第一个quiz
            var quiz = wordQuizzesQueue.Dequeue();
            wordPage.updateWordQuiz(quiz);
            
            // 绑定事件，取栈内第一个quiz
            wordPage.NextQuizEvent += () =>
            {
                
                if(!wordPage.lastAnswerCorrect())
                {
                    // 如果上一题答错，则在更新前重新压入quiz进队列
                    wordQuizzesQueue.Enqueue(quiz);
                }
                else
                {
                    // 答对则加分
                    pointsManager.changePoints(1);
                }
                
                updateProgress(quiz, wordPage.lastAnswerCorrect());

                if (wordQuizzesQueue.Count <= 0)
                {
                    // 关闭UI
                    Game1.exitActiveMenu();

                    Game1.drawObjectDialogue("单词石碑已经摧毁！");

                    statisticsManager.statisticsState.normalLevelCount++;
                    // 传送到楼梯层
                    levelManager.onCallStairLevel();

                    return;
                }
                
                wordPage.updateQuizCount();
                quiz = wordQuizzesQueue.Dequeue();
                wordPage.updateWordQuiz(quiz);
            };

            Game1.activeClickableMenu = wordPage;
        }

        public void updateProgress(Word word, bool answerIsCorrect)
        {
            if (!answerIsCorrect)
                word.wrongCount++;
            word.isLearned = true;
        }
        /*
        public void updateProgress(List<WordQuiz> wordQuizList)
        {
            // TODO: 这里逻辑是学完“一组”单词统一更新进度
            // 考虑是否要学一个更新一个

            foreach (WordQuiz wordQuiz in wordQuizList)
            {
                wordsManager.progress.wordsState[wordQuiz.quizText].isLearned = true;
                wordsManager.progress.wordsLearnedCount ++;
                wordsManager.progress.wordsUnlearnedIndex++;
            }
        }
        */


    }
}
