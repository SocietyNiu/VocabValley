using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Force.DeepCloner;
using StardewValley.Objects;
using Microsoft.Xna.Framework.Input;
using VocabValley.Utils;
using VocabValley.Core.Model;

namespace VocabValley.UI
{
    internal class BossPage: WordLearningPage
    {
        // 决定Quiz类型
        // 目前0表示英文题目，1表示中文题目
        private int quizType = 0;
        private Random random;

        public BossPage(IModHelper helper, IMonitor monitor,
            WordsManager wordsManager)
            : base(helper, monitor, wordsManager)
        {
            random = new Random();
        }
        public override void updateWordQuiz(Word word)
        {
            // 执行基类shuffle选项代码，但是重新创建选项框
            base.updateWordQuiz(word);

            quizType = random.Next(2);
            boxes.Clear();
            // 创建选项复选框
            // TODO: 复选框whichOption重复问题
            for (int i = 0; i < targetWord.options.Count(); i++)
            {
                var box = new OptionsCheckbox(
                    quizType == 0? wordsManager.getByID(targetWord.options[i]).translation:
                    wordsManager.getByID(targetWord.options[i]).text,
                    i + 866,
                    xPositionOnScreen + 150,
                    yPositionOnScreen + 150 + 50 * i
                );
                boxes.Add(box);
            }
        }

        public override void drawAnswer(SpriteBatch b)
        {
            // showAnswer == 1 答案正确
            if (showAnswer == 1)
            {
                b.DrawString(
                Game1.dialogueFont,
               "答案正确",
               new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 360),
               Color.Green,
               rotation: 0f,
               origin: Vector2.Zero,
               scale: 1.0f,
               effects: SpriteEffects.None,
               layerDepth: 1f
                );
            }
            // showAnswer == 2 答案错误
            else if (showAnswer == 2)
            {
                b.DrawString(
                Game1.dialogueFont,
               "答案错误\n正确答案:" + 
               (quizType ==0? targetWord.translation: targetWord.text),
               new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 360),
               Color.Red,
               rotation: 0f,
               origin: Vector2.Zero,
               scale: 1.0f,
               effects: SpriteEffects.None,
               layerDepth: 1f
                );
            }
        }

        public override void drawTitle(SpriteBatch b)
        {
            b.DrawString(
            Game1.dialogueFont,
            quizType == 0? this.targetWord.text:this.targetWord.translation,
                new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 75),
                Color.Black,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1.0f,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );
        }
    }
}
