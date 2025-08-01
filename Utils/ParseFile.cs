using StardewModdingAPI;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;

namespace VocabValley.Utils
{
    internal class ParseFile
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public ParseFile(IModHelper helper, IMonitor monitor) 
        {
            Helper = helper;
            Monitor = monitor;
        }
        public string[]? ReadFile(string fileName)
        {

            string filePath = Path.Combine(Helper.DirectoryPath, "vocabulary", fileName);
                
            if (!File.Exists(filePath))
            {
                Monitor.Log("文件不存在", LogLevel.Error);
                return null;
            }
            
            try
            {
                string[] _words = File.ReadAllLines(filePath);
                Monitor.Log($"成功加载 {_words.Length} 条单词", LogLevel.Info);
                return _words;
            }
            catch (Exception ex)
            {
                Monitor.Log($"读取单词文件出错：{ex}", LogLevel.Error);
                return null;
            }
        }

        public List<Word> parseWords(string[] _words)
        {
            return _words.Where(line => !string.IsNullOrWhiteSpace(line))     // 跳过空行
                    .Select(line => line.Split(new[] { '\t' }, 2))      // 按第一个制表符分成两部分
                    .Where(parts => parts.Length == 2)                  // 确保格式正确
                    .Select((parts, idx) => new Word
                    {
                        ID = idx,
                        text = parts[0].Trim(),
                        translation = parts[1].Trim(),
                        isLearned = false,
                        reviewCount = 0
                    })
                    .ToList();
        }

    }
}
