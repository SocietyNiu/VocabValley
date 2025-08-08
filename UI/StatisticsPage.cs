using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VocabValley.Utils;
using VocabValley.Core.Model;

namespace VocabValley.UI
{
    internal class StatisticsPage : IClickableMenu
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private const string BGTEXTURE = "LooseSprites/VocabValley_EmptyPage";
        private readonly Texture2D bgTexture;
        private StatisticsState statisticsState;
        private WordsManager wordsManager;

        public StatisticsPage(IModHelper helper, IMonitor monitor, StatisticsState statisticsState, WordsManager wordsManager) :
            base(x: (Game1.uiViewport.Width - 800) / 2,
                 y: (Game1.uiViewport.Height - 600) / 2,
                 width: 800,
                 height: 600,
                 true)
        {
            this.Helper = helper;
            this.Monitor = monitor;
            this.statisticsState = statisticsState;
            this.wordsManager = wordsManager;

            bgTexture = helper.GameContent.Load<Texture2D>(BGTEXTURE);
            
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
            b.DrawString(
                Game1.dialogueFont,
                "当前词库的统计数据",
                UIUtils.textCenterAlignedPos("当前词库的统计数据",
                new Vector2(xPositionOnScreen, yPositionOnScreen + 50),
                new Vector2(800, 50)
                ),
                Color.Black
            );

            // 花费的时间
            b.DrawString(Game1.dialogueFont, "你学习的总时长为：", new Vector2(xPositionOnScreen + 100, yPositionOnScreen + 100), Color.Black);
            b.DrawString(Game1.dialogueFont, CustomTool.Time2String(statisticsState.totalSeconds), new Vector2(xPositionOnScreen + 500, yPositionOnScreen + 100), Color.Red);

            // 普通层
            b.DrawString(Game1.dialogueFont, "你摧毁的单词石碑数量为：", new Vector2(xPositionOnScreen + 100, yPositionOnScreen + 150), Color.Black);
            b.DrawString(Game1.dialogueFont, statisticsState.normalLevelCount.ToString(), new Vector2(xPositionOnScreen + 500, yPositionOnScreen + 150), Color.Red);
            base.draw(b);

            // Boss层
            b.DrawString(Game1.dialogueFont, "你打败的单词守护者数量为：", new Vector2(xPositionOnScreen + 100, yPositionOnScreen + 200), Color.Black);
            b.DrawString(Game1.dialogueFont, statisticsState.BossLevelCount.ToString(), new Vector2(xPositionOnScreen + 500, yPositionOnScreen + 200), Color.Red);
            base.draw(b);

            // 学习进度
            b.DrawString(Game1.dialogueFont, "当前词书的进度（已学单词/所有单词）：", new Vector2(xPositionOnScreen + 100, yPositionOnScreen + 250), Color.Black);
            b.DrawString(Game1.dialogueFont, wordsManager.getProgress(), new Vector2(xPositionOnScreen + 500, yPositionOnScreen + 300), Color.Red);
            base.draw(b);

            drawMouse(b);
        }
    }
}
