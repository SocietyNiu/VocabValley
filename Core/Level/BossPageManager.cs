using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.UI;
using VocabValley.Utils;

namespace VocabValley.Core.Level
{
    /// <summary>
    /// Boss 战本质是特殊的单词学习页面
    /// 增加计时器、修改单词范围、改变结束条件等
    /// </summary>
    internal class BossPageManager

    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly WordsManager wordsManager;
        private readonly LevelManager levelManager;


        // 答错不重新学习，直接结束
        private Queue<Word> wordQuizzesQueue;

        private int wrongCount = 0;
        private CountDown countDown = null;

        public BossPageManager(IModHelper helper, IMonitor monitor,
            WordsManager wordsManager,
            int countDown, LevelManager levelManager)
        {
            Helper = helper;
            Monitor = monitor;
            this.wordsManager = wordsManager;
            this.levelManager = levelManager;

            wrongCount = 0;

            if (countDown > 0)
            {
                this.countDown = new CountDown(helper, monitor, countDown);
            }

            
        }

        public List<Word> initialization()
        {
            try
            {
                return wordsManager.getUnreviewedWords();
            }
            catch (Exception ex)
            {
                Monitor.Log(ex.Message, LogLevel.Error);
                return new List<Word>();
            }
        }

        public void onBossPageCall()
        {
            var quizList = initialization();

            if (quizList.Count <= 0)
            {
                Game1.drawObjectDialogue("没有需要复习的单词！");
                return;
            }

            // 压入所有的quiz
            wordQuizzesQueue = new Queue<Word>();
            foreach (Word wordQuiz in quizList)
            {
                wordQuizzesQueue.Enqueue(wordQuiz);
            }

            BossPage wordPage = new BossPage(Helper, Monitor, wordsManager);

            wordPage.setQuizCount(wordQuizzesQueue.Count());

            // 展示第一个quiz
            var quiz = wordQuizzesQueue.Dequeue();
            wordPage.updateWordQuiz(quiz);

            wordPage.NextQuizEvent += () =>
            {
                // 如果上一题答错，则记录
                if (!wordPage.lastAnswerCorrect())
                {
                    wrongCount++;
                }
                updateProgress(quiz, wordPage.lastAnswerCorrect());

                if (wordQuizzesQueue.Count <= 0)
                {
                    // 关闭UI
                    Game1.exitActiveMenu();

                    Game1.drawObjectDialogue("单词守护者已被击倒！");

                    // 传送到楼梯层
                    levelManager.onCallRewardLevel();

                    return;
                }
                wordPage.nowCount++;
                quiz = wordQuizzesQueue.Dequeue();
                wordPage.updateWordQuiz(quiz);
            };
            Game1.activeClickableMenu = wordPage;
            Helper.Events.Display.MenuChanged += OnMenuChanged;

            // 开始计时
            if (countDown != null)
            {
                countDown.updateCountDownEvent += () =>
                {
                    if (countDown.remainSeconds <= 0)
                    {
                        Game1.exitActiveMenu();
                        Game1.drawObjectDialogue("时间结束");
                        return;
                    }
                    wordPage.countDown = countDown.getCountDown();
                };

                countDown.start();
            }

        }
        public void updateProgress(Word word, bool answerIsCorrect)
        {
            if (!answerIsCorrect)
                word.wrongCount++;
            word.reviewCount++;
        }

        void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // 旧菜单是WordLearningPage，且新菜单不是它（可能是 null）
            // 则销毁计时器并解绑事件
            if (e.OldMenu is WordLearningPage && !(e.NewMenu is WordLearningPage) && countDown != null)
            {
                countDown.stop();                              // 销毁计时器
                Helper.Events.Display.MenuChanged -= OnMenuChanged; // 解绑自身，防泄漏
            }
        }
    }
}
