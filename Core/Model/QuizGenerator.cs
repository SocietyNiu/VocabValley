using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabValley.Core.Model
{
    internal class QuizGenerator
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public List<Word> wordList;
        private readonly Random rng = new Random();

        public QuizGenerator(IModHelper helper, IMonitor monitor, List<Word> WordList)
        {
            Helper = helper;
            Monitor = monitor;
            wordList = WordList;
        }
        
        public void randomGenerateQuiz()
        {
           // 完善异常体系
            if(wordList.Count < 4)
            {
                throw new InvalidOperationException("单词列表中的单词数量不足以生成测验");
            }

            foreach(var w in wordList)
            {
                // 使用哈希集来存储选项，避免重复
                // 先把自己的ID加入选项中
                HashSet<int> opts = new HashSet<int> { w.ID};

                while(opts.Count < 4)
                {
                    int candidate = wordList[rng.Next(wordList.Count)].ID;
                    if (candidate != w.ID)        // 排除自己
                        opts.Add(candidate);
                }

                // 这里不必打乱，在出题时打乱，防止每题顺序都一样

                w.options = opts.ToList();
            }
        }

        /// 以下是Word和WordQuiz分离时的Quiz生成方式
        /*
        // wordIndex 为-1时，随机选择单词作为目标
        public WordQuiz RandomGenerateOneQuiz(int id, int wordIndex = -1)
        {
            // TODO: 完善异常体系  
            if (wordList.Count < 4)
            {
                throw new InvalidOperationException("单词列表中的单词数量不足以生成测验");
            }

            Random random = new Random();

            var quizWord = wordList[wordIndex == -1 ? random.Next(wordList.Count) : wordIndex];

            // 添加答案到选项  
            var options = new List<string> { quizWord.translation };

            // 随机添加干扰项  
            while (options.Count < 4)
            {
                var randomWord = wordList[random.Next(wordList.Count)].translation;
                if (!options.Contains(randomWord))
                {
                    options.Add(randomWord);
                }
            }

            // 打乱选项顺序  
            options = options.OrderBy(x => random.Next()).ToList();
            int answerIndex = options.IndexOf(quizWord.translation);

            return new WordQuiz(id, quizWord.text, options, answerIndex);
        }

        public List<WordQuiz> RandomGenerateAllQuiz(int idStart)
        {
            var wordQuizList = new List<WordQuiz> { };
            for (int wordIndex = 0; wordIndex < wordList.Count() ; wordIndex++)
            {
                wordQuizList.Add(
                    RandomGenerateOneQuiz(idStart++, wordIndex));
            }
            return wordQuizList;
        }*/
    }
}
