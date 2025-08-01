using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Utils;
using static StardewValley.Menus.CharacterCustomization;

namespace VocabValley.Core.Model
{
    internal class WordsManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly ParseFile parseFile;

        public List<Word> wordsList;

        public WordsManager(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;
            parseFile = new ParseFile(Helper, Monitor);
        }

        public void getWordsFromFile(string filename)
        {
            var words = parseFile.ReadFile(filename);
            wordsList = parseFile.parseWords(words ?? Array.Empty<string>());

        }
        public void initialization()
        {

            if (wordsList == null)
            {
                Monitor.Log("Warning: wordsList is null during initialization", LogLevel.Warn);
                return;
            }
            QuizGenerator quizGenerator = new QuizGenerator(Helper, Monitor, wordsList);
            quizGenerator.randomGenerateQuiz();
        }
        public List<Word> getUnlearnedWordsInOrder(int count)
        {
            if(wordsList == null)
            {
                throw new ArgumentNullException("Error: wordsList is null during getUnlearnedWordsInOrder");
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("Error: Negative count during getUnlearnedWordsInOrder");
            }
            
            return wordsList
                .Where(Word => Word.isLearned == false)
                .OrderBy(Word => Word.ID)
                .Take(count)
                .ToList();
        }

        public Word getByID(int id)
        {
            if (wordsList == null)
            {
                throw new ArgumentNullException("Error: wordsList is null during getByID");
            }
            return wordsList.FirstOrDefault(word => word.ID == id) ?? throw new KeyNotFoundException($"Word with ID {id} not found.");
        }

        public List<Word> getLearnedWords()
        {
            if (wordsList == null)
            {
                throw new ArgumentNullException("Error: wordsList is null during getLearnedWords");
            }
            return wordsList
                   .Where(word => word.isLearned)
                   .ToList();
        }
        
        public List<Word> getLearnedWordsByRandom(int count)
        {
            return getLearnedWords()
                .OrderBy(x => Guid.NewGuid())
                .Take(count)
                .ToList();
        }
        
        public List<Word> getLearnedWordsByWrongTimes()
        {
            return getLearnedWords()
                   .OrderByDescending(word => word.wrongCount)
                   .ToList();
        }

        public List<Word> getWrongWords()
        {
            return wordsList
                   .Where(word => word.wrongCount > 0)
                   .OrderByDescending(word => word.wrongCount)
                   .ToList();
        }

        public List<Word> getUnreviewedWords()
        {
            // 返回学习过但是还没复习的单词
            return wordsList
                   .Where(word => word.reviewCount == 0 && word.isLearned )
                   .ToList();
        }
    }
}
