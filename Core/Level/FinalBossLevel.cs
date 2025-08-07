using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Statistics;
using VocabValley.UI;
using Microsoft.Xna.Framework;
using VocabValley.Utils;
using StardewModdingAPI.Events;
using VocabValley.Core.Saving;

namespace VocabValley.Core.Level
{
    internal class FinalBossLevel
    {
        /// <summary>
        /// 在最终层，共有五个Boss，分别是黑化的五位村民
        /// 用progress来表示当前Boss战的进度
        /// 从左到右进度依次增加，0表示未开始，5表示全部Boss已被击败
        /// </summary>
        /// 

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly WordsManager wordsManager;
        private readonly LevelManager levelManager;

        private Queue<Word> wordQuizzesQueue;
        private CountDown? countDown = null;

        public FinalProgress finalProgress;

        private string[] NPC = { "阿比盖尔", "塞巴斯蒂安", "艾米丽", "谢恩", "海莉" };

        public FinalBossLevel(IModHelper helper, IMonitor monitor, WordsManager wordsManager, LevelManager levelManager, FinalProgress finalProgress)
        {
            Helper = helper;
            Monitor = monitor;
            this.wordsManager = wordsManager;
            this.levelManager = levelManager;
            this.finalProgress = finalProgress;

            GameLocation.RegisterTileAction("onFinalBossPageCall", onFinalBossPageCall);
        }

        
        public bool onFinalBossPageCall(GameLocation location, string[] args, Farmer player, Point point)
        {
            if (!int.TryParse(args[1], out int choose))
            {
                return true;
            }

            if(choose > finalProgress.Progress)
            {
                Game1.drawObjectDialogue($"你还没通过{NPC[finalProgress.Progress]}的试炼");
                return true;
            }
            if(choose < finalProgress.Progress)
            {
                Game1.drawObjectDialogue($"你已经通过了{NPC[choose]}的试炼");
                return true;
            }

            switch (choose)
            {
                case 0:
                    mission1(); break;
                case 1:
                    mission2(); break;
                case 2:
                    mission3(); break;
                case 3:
                    mission4(); break;
                case 4:
                    mission5(); break;
                default:
                    return true;
            }
            return true;
        }

        public BossPage generatMission(List<Word> quizList, Action? nextQuizEvent)
        {
            wordQuizzesQueue = new Queue<Word>();
            foreach (Word wordQuiz in quizList)
            {
                wordQuizzesQueue.Enqueue(wordQuiz);
            }
            BossPage wordPage = new BossPage(Helper, Monitor, wordsManager);

            wordPage.setQuizCount(wordQuizzesQueue.Count);

            var quiz = wordQuizzesQueue.Dequeue();
            wordPage.updateWordQuiz(quiz);

            wordPage.NextQuizEvent += nextQuizEvent;
            Game1.activeClickableMenu = wordPage;

            // 如果倒计时被构造了
            if(countDown != null)
            {
                var cd = countDown;
                cd.updateCountDownEvent += () =>
                {
                    if (cd.remainSeconds <= 0)
                    {
                        Game1.exitActiveMenu();
                        Game1.drawObjectDialogue("时间结束，你没有通过试炼");
                        return;
                    }
                    wordPage.countDown = cd.getCountDown();
                };

                cd.start();
            }

            return wordPage;
        }
        public void mission1()
        {
            /// 任务一
            /// 错词频率前50
            List<Word> quizList = wordsManager.getWrongWords(50);
            int totalWordsCount = quizList.Count;
            int wrongCount = 0;

            BossPage wordPage = null;
            Action nextQuizEvent = () =>
            {
                if (!wordPage.lastAnswerCorrect())
                {
                    wrongCount++;
                }

                if (wordQuizzesQueue.Count <= 0)
                {
                    double currency = (double)(totalWordsCount - wrongCount)/totalWordsCount;
                    
                    // 关闭UI
                    Game1.exitActiveMenu();
                    if (currency < 0.8)
                    {
                        Game1.drawObjectDialogue($"你未能通过阿比盖尔的试炼，你的正确率为{currency:F2}。");
                    }
                    else
                    {
                        finalProgress.Progress++;
                        Game1.drawObjectDialogue($"干得漂亮，你已经通过了阿比盖尔的试炼。你的正确率为{currency:F2}。");
                    }
                    return;
                }
                wordPage.nowCount++;
                var quiz = wordQuizzesQueue.Dequeue();
                wordPage.updateWordQuiz(quiz);
            };

            wordPage = generatMission(quizList, nextQuizEvent);
        }

        public void mission2() 
        {
            /// 任务二：
            /// 从所有错题中任意抽取，限时答对正确率60%以上
            /// 一道题平均4秒

            List<Word> quizList = wordsManager.getWrongWordsRandomly(50);
            int totalWordsCount = quizList.Count;
            int totalTime = totalWordsCount * 4;
            int wrongCount = 0;
            int answeredCount = 0;

            if (totalTime > 0)
                this.countDown = new CountDown(Helper, Monitor, totalTime);

            BossPage wordPage = null;
            Action nextQuizEvent = () =>
            {
                answeredCount++;
                if (!wordPage.lastAnswerCorrect())
                {
                    wrongCount++;
                }

                if (wordQuizzesQueue.Count <= 0)
                {

                    if (!(countDown is null))
                    {
                        countDown.stop();
                        countDown = null;
                    }

                    double currency = (double)(answeredCount - wrongCount) / answeredCount;

                    // 关闭UI
                    Game1.exitActiveMenu();
                    if (currency < 0.6)
                    {
                        Game1.drawObjectDialogue($"你未能通过塞巴斯蒂安的试炼，你的正确率为{currency:F2}。");
                    }
                    else
                    {
                        finalProgress.Progress++;
                        Game1.drawObjectDialogue($"干得漂亮，你已经通过了塞巴斯蒂安的试炼。你的正确率为{currency:F2}。");
                    }
                    return;
                }
                wordPage.nowCount++;
                var quiz = wordQuizzesQueue.Dequeue();
                wordPage.updateWordQuiz(quiz);
            };

            wordPage = generatMission(quizList, nextQuizEvent);

        }

        public void mission3()
        {
            /// 任务三：
            /// 从所有词库中抽题，要求百分百准确率

            List<Word> quizList = wordsManager.getLearnedWordsByRandom(20);

            BossPage wordPage = null;
            Action nextQuizEvent = () =>
            {
                if (!wordPage.lastAnswerCorrect())
                {
                    Game1.exitActiveMenu();
                    Game1.drawObjectDialogue($"你未能通过艾米丽的试炼。");
                }

                if (wordQuizzesQueue.Count <= 0)
                {
                    finalProgress.Progress++;
                    Game1.exitActiveMenu();
                    Game1.drawObjectDialogue($"干得漂亮，你已经通过了艾米丽的试炼。");
                    return;
                }

                wordPage.nowCount++;
                var quiz = wordQuizzesQueue.Dequeue();
                wordPage.updateWordQuiz(quiz);
            };

            wordPage = generatMission(quizList, nextQuizEvent);

        }

        public void mission4()
        {
            /// 任务四：
            /// 从所有题中任意抽取，限时答对正确率70%以上
            /// 一道题平均4秒
            List<Word> quizList = wordsManager.getLearnedWordsByRandom(50);
            int totalWordsCount = quizList.Count;
            int totalTime = totalWordsCount * 4;
            int wrongCount = 0;
            int answeredCount = 0;

            if (totalTime > 0)
                this.countDown = new CountDown(Helper, Monitor, totalTime);

            BossPage wordPage = null;
            Action nextQuizEvent = () =>
            {
                answeredCount++;
                if (!wordPage.lastAnswerCorrect())
                {
                    wrongCount++;
                }

                if (wordQuizzesQueue.Count <= 0)
                {

                    if (!(countDown is null))
                    {
                        countDown.stop();
                        countDown = null;
                    }

                    double currency = (double)(answeredCount - wrongCount) / answeredCount;

                    // 关闭UI
                    Game1.exitActiveMenu();
                    if (currency < 0.7)
                    {
                        Game1.drawObjectDialogue($"你未能通过谢恩的试炼，你的正确率为{currency:F2}。");
                    }
                    else
                    {
                        finalProgress.Progress++;
                        Game1.drawObjectDialogue($"干得漂亮，你已经通过了谢恩的试炼。你的正确率为{currency:F2}。");
                    }
                    return;
                }
                wordPage.nowCount++;
                var quiz = wordQuizzesQueue.Dequeue();
                wordPage.updateWordQuiz(quiz);
            };

            wordPage = generatMission(quizList, nextQuizEvent);
        }

        public void mission5()
        {
            /// 任务四：
            /// 从所有题中任意抽取，限时答对正确率90%以上
            /// 一道题平均8秒

            List<Word> quizList = wordsManager.getLearnedWordsByRandom(120);
            int totalWordsCount = quizList.Count;
            int totalTime = totalWordsCount * 8;
            int wrongCount = 0;
            int answeredCount = 0;

            if (totalTime > 0)
                this.countDown = new CountDown(Helper, Monitor, totalTime);

            BossPage wordPage = null;
            Action nextQuizEvent = () =>
            {
                answeredCount++;
                if (!wordPage.lastAnswerCorrect())
                {
                    wrongCount++;
                }

                if (wordQuizzesQueue.Count <= 0)
                {

                    if (!(countDown is null))
                    {
                        countDown.stop();
                        countDown = null;
                    }

                    double currency = (double)(answeredCount - wrongCount) / answeredCount;

                    // 关闭UI
                    Game1.exitActiveMenu();
                    if (currency < 0.9)
                    {
                        Game1.drawObjectDialogue($"你未能通过海莉的试炼，你的正确率为{currency:F2}。");
                    }
                    else
                    {
                        finalProgress.Progress++;
                        finalProgress.IsEnd = true;
                        Game1.drawObjectDialogue($"干得漂亮，你已经通过了海莉的试炼。你的正确率为{currency:F2}。");
                        levelManager.onCallFinalRewardLevel();
                    }
                    return;
                }
                wordPage.nowCount++;
                var quiz = wordQuizzesQueue.Dequeue();
                wordPage.updateWordQuiz(quiz);
            };

            wordPage = generatMission(quizList, nextQuizEvent);
        }
        void OnMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            // 旧菜单是BossPage，且新菜单不是它（可能是 null）
            // 则销毁计时器并解绑事件
            if (e.OldMenu is BossPage && !(e.NewMenu is BossPage) && countDown != null)
            {
                // 销毁计时器
                countDown.stop();
                countDown = null;
                Helper.Events.Display.MenuChanged -= OnMenuChanged; // 解绑自身，防泄漏
            }
        }
    }
}
