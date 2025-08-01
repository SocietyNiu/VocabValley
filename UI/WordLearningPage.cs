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
    /// <summary>
    /// UI只负责显示题目，不负责任何题目生成逻辑处理
    /// </summary>
    internal class WordLearningPage : IClickableMenu
    {

        // TODO: 选择答案后则禁止监听鼠标
        private const string BGTEXTURE = "LooseSprites/VocabValley_EmptyPage";
        private const string BTTEXTURE = "LooseSprites/VocabValley_Button";
        private const string ALARMTEXTURE = "LooseSprites/VocabValley_Alarm";

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        // 背景图片材质和按钮图片材质
        private readonly Texture2D bgTexture;
        private readonly Texture2D btTexture;
        private readonly Texture2D alarmTexture;

        protected readonly WordsManager wordsManager;
        protected Word targetWord;

        protected List<OptionsCheckbox> boxes = new();

        protected int showAnswer = 0;
        private bool answered = false;

        private readonly ClickableTextureComponent nextButton;
        private readonly string nextButtonLabel;

        // 用于暴露给外层的下一题事件
        public event Action? NextQuizEvent;
        // 摧毁计时器事件
        public event Action? CountDownExitEvent;

        // Quiz数及剩余数量
        public int allCount;
        public int nowCount;

        public string countDown = null;

        // TODO: 自定义调节窗口大小
        // TODO: 调整窗口大小时动态调整UI
        public WordLearningPage(IModHelper helper, IMonitor monitor, WordsManager wordsManager) :
            base(x: (Game1.uiViewport.Width - 800) / 2,
                 y: (Game1.uiViewport.Height - 600) / 2,
                 width: 800,
                 height: 600,
                 true)
        {
            Helper = helper;
            Monitor = monitor;
            this.wordsManager = wordsManager;

            bgTexture = helper.GameContent.Load<Texture2D>(BGTEXTURE);
            btTexture = helper.GameContent.Load<Texture2D>(BTTEXTURE);
            alarmTexture = helper.GameContent.Load<Texture2D>(ALARMTEXTURE);

            nextButton = new ClickableTextureComponent(
                name: "NextAnswer",
                bounds: new Rectangle(xPositionOnScreen + 560, yPositionOnScreen + 450, 240, 120),
                label: "",
                hoverText: "",
                texture: btTexture,
                sourceRect: new Rectangle(0, 0, 240, 120),
                scale: 0.5f);
            nextButtonLabel = "下一题";
            
        }

        public virtual void updateWordQuiz(Word word)
        {
            answered = false;
            showAnswer = 0;

            targetWord = word.DeepClone();

            // 洗牌，打乱选项顺序
            Random rng = new();
            for (int i = targetWord.options.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (targetWord.options[i], targetWord.options[j]) =
                    (targetWord.options[j], targetWord.options[i]);
            }

            boxes.Clear();
            // 创建选项复选框
            // TODO: 复选框whichOption重复问题
            for (int i = 0; i < targetWord.options.Count(); i++)
            {
                var box = new OptionsCheckbox(
                    wordsManager.getByID(targetWord.options[i]).translation,
                    i+866,
                    xPositionOnScreen + 150, 
                    yPositionOnScreen + 150 + 50 * i
                );
                boxes.Add(box);
            }

        }

        public override void draw(SpriteBatch b)
        {
            // 画遮罩层
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.5f);

            // 画背景图片
            b.Draw(bgTexture, new Vector2(xPositionOnScreen, yPositionOnScreen),
               sourceRectangle: new Rectangle(0, 0, width, height),
               color: Color.White, origin: Vector2.Zero, layerDepth: 0.8f,
               scale: 1f, effects: SpriteEffects.None, rotation: 0.0f);

            // 画题目
            drawTitle(b);

            // 画选项复选框
            foreach (var elem in boxes)
            {
                elem.draw(b, 0, 0);
            }

            // 画答案
            drawAnswer(b);

            if(answered==true)
                drawButton(b, nextButton, nextButtonLabel);

            // 画页码
            b.DrawString(
            Game1.smallFont,
                nowCount.ToString() + "/" + allCount.ToString(),
                new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 460),
                Color.Black,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1.0f,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );

            // 画计时器
            drawAlarm(b);

            // 画主体内容
            base.draw(b);

            // 画鼠标
            drawMouse(b);
        }
        public virtual void drawTitle(SpriteBatch b)
        {
            b.DrawString(
            Game1.dialogueFont,
                this.targetWord.text,
                new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 75),
                Color.Black,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1.0f,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );
        }
        
        public virtual void drawAnswer(SpriteBatch b)
        {
            // showAnswer == 1 答案正确
            if(showAnswer==1)
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
            else if (showAnswer==2)
            {
                b.DrawString(
                Game1.dialogueFont,
               "答案错误\n正确答案:" + targetWord.translation,
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

        private void drawAlarm(SpriteBatch b)
        {
            if (countDown == null)
                return;
            float xpos, ypos, width, height;
            
            width = 128;
            height = 128;
            xpos = 50;
            ypos = 50;

            // 计算文本的大小
            Vector2 size = Game1.dialogueFont.MeasureString(countDown);

            // 计算文本的位置
            Vector2 pos = new Vector2(
                xpos + (width  - size.X) / 2f,
                ypos + (height  - size.Y) / 2f
            );


            b.Draw(alarmTexture, new Vector2(xpos, ypos),
               sourceRectangle: new Rectangle(0, 0, 128, 128),
               color: Color.White, origin: Vector2.Zero, layerDepth: 0.8f,
               scale: 1f, effects: SpriteEffects.None, rotation: 0.0f);

            b.DrawString(
            Game1.dialogueFont,
                countDown,
                pos,
                Color.White,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1.0f,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );

        }

        // 自行实现按键文字绘制
        private void drawButton(SpriteBatch b, ClickableTextureComponent buttonInfo, string buttonLabel)
        {
            // 画按键
            buttonInfo.draw(b);

            // 计算文本的大小
            Vector2 size = Game1.dialogueFont.MeasureString(buttonLabel);

            // 计算文本的位置
            Vector2 pos = new Vector2(
                buttonInfo.bounds.X + (buttonInfo.bounds.Width / 2f - size.X) / 2f,
                buttonInfo.bounds.Y + (buttonInfo.bounds.Height / 2f - size.Y) / 2f
            );

            // 绘制文字
            b.DrawString(
                Game1.dialogueFont,
                buttonLabel,
                pos,
                Color.Black,
                rotation: 0f,
                origin: Vector2.Zero,
                scale: 1.0f,
                effects: SpriteEffects.None,
                layerDepth: 1f
            );
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {

            // 已经回答则解除监听            
            if(answered == false)
            {
                // 检查鼠标点击位置是否在复选框区域内
                foreach (var cb in boxes)
                {
                    // 只有鼠标落在这个 Checkbox 区域内才转发点击
                    if (cb.bounds.Contains(x, y))
                    {
                        cb.receiveLeftClick(x, y);
                        checkAnswer();
                        this.answered = true;       // 解除监听
                        break;                 // 命中一个就够了，省得多次处理
                    }
                }

            }
            if(answered == true)
            {
                if (nextButton.containsPoint(x, y))
                {
                    NextQuizEvent?.Invoke();
                    // TODO: 添加音效
                }
            }
            

            base.receiveLeftClick(x, y, playSound);
        }
        public void checkAnswer()
        {
            if (boxes[targetWord.options.IndexOf(targetWord.ID)].isChecked)
            {
                this.showAnswer = 1;
            }
            else this.showAnswer = 2;
        }

        public bool lastAnswerCorrect()
        {
            return this.showAnswer == 1;
        }

        public void setQuizCount(int n)
        {
            this.allCount = n;
            this.nowCount = 1;
        }
        public void updateQuizCount()
        {
            if(showAnswer == 1)
            {
                nowCount++;
                return;
            }
            // 如果答案错误，需要向队列内压入该错题
            if(showAnswer ==2 )
            {
                nowCount++;
                allCount++;
                return;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);

            // 已经回答则解除监听            
            if (answered == false)
            {
                int idx = CustomTool.Key2Int(key);
                if (idx >= 0 && idx < boxes.Count)
                {
                    // 触发和鼠标一样的逻辑
                    // 坐标无所谓，只要让 SMAPI 认为点击生效即可
                    boxes[idx].receiveLeftClick(0, 0);

                    checkAnswer();
                    answered = true;
                    Game1.playSound("smallSelect");
                    return;
                }
            }
            else
            {
                int idx = CustomTool.Key2Int(key);
                if(idx == 88)
                {
                    NextQuizEvent?.Invoke();
                }
            }
        }
    }
}
