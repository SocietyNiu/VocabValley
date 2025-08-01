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
    internal class WrongWordPage : IClickableMenu
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private const string BGTEXTURE = "LooseSprites/VocabValley_EmptyPage";
        private const string CURSORTEXTURE = "LooseSprites/Cursors";

        private readonly Texture2D bgTexture;
        private readonly Texture2D cursorTexture;

        private readonly ClickableTextureComponent leftButton;
        private readonly ClickableTextureComponent rightButton;

        private List<Word> words;
        public int currentPage;
        public int totalPage;


        public Action? LastPage;
        public Action? NextPage;


        public WrongWordPage(IModHelper helper, IMonitor monitor):
            base(x: (Game1.uiViewport.Width - 800) / 2,
                 y: (Game1.uiViewport.Height - 600) / 2,
                 width: 800,
                 height: 600,
                 true)
        {
            Helper = helper;
            Monitor = monitor;

            bgTexture = helper.GameContent.Load<Texture2D>(BGTEXTURE);
            cursorTexture = helper.GameContent.Load<Texture2D>(CURSORTEXTURE);

            rightButton = new ClickableTextureComponent(
                name: "LastPage",
                bounds: new Rectangle(xPositionOnScreen+width-128, yPositionOnScreen + height - 112, 64, 64),
                label: "",
                hoverText: "",
                texture: cursorTexture,
                sourceRect: new Rectangle(0, 192, 64, 64),
                scale: 1.0f
                );
            leftButton = new ClickableTextureComponent(
                name: "NextPage",
                bounds: new Rectangle(xPositionOnScreen+64, yPositionOnScreen+height-112, 64, 64),
                label: "",
                hoverText: "",
                texture: cursorTexture,
                sourceRect: new Rectangle(0, 256, 64, 64),
                scale: 1.0f
                );
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

            if(currentPage != 1)
                leftButton.draw(b);

            if(currentPage != totalPage)
                rightButton.draw(b);    

            b.DrawString(
                Game1.dialogueFont,
                "单词",
                UIUtils.textCenterAlignedPos("单词",
                new Vector2(xPositionOnScreen+ 50, yPositionOnScreen+ 50),
                new Vector2(180, 50)
                ),
                Color.Black);

            b.DrawString(
                Game1.dialogueFont,
                "释义",
                UIUtils.textCenterAlignedPos("释义",
                new Vector2(xPositionOnScreen + 230, yPositionOnScreen + 50),
                new Vector2(400, 50)
                ),
                Color.Black);

            b.DrawString(
                Game1.dialogueFont,
                "错误次数",
                UIUtils.textCenterAlignedPos("错误次数",
                new Vector2(xPositionOnScreen + 630, yPositionOnScreen + 50),
                new Vector2(120, 50)
                ),
                Color.Black);

            // 画页码
            string pageString = currentPage.ToString()+'/'+totalPage.ToString();
            b.DrawString(
                Game1.dialogueFont,
                pageString,
                UIUtils.textCenterAlignedPos(pageString,
                new Vector2(xPositionOnScreen + 50, yPositionOnScreen + height - 100),
                new Vector2(700, 50)
                ),
                Color.Black);

            // 画错词
            for(int i = 0; i < words.Count(); i++)
            {
                drawWord(b, words[i], i);
            }

            base.draw(b);
            // 画鼠标
            drawMouse(b);
        }
        public void drawWord(SpriteBatch b, Word word, int index)
        {
            int relativeY = index * 50 + 100 + yPositionOnScreen;
            b.DrawString(
                Game1.dialogueFont,
                word.text,
                UIUtils.textCenterAlignedPos(word.text,
                new Vector2(xPositionOnScreen + 50, relativeY),
                new Vector2(180, 50)
                ),
                Color.Black);

            b.DrawString(
                Game1.dialogueFont,
                word.translation,
                UIUtils.textCenterAlignedPos(word.translation,
                new Vector2(xPositionOnScreen + 230, relativeY),
                new Vector2(400, 50)
                ),
                Color.Black);

            b.DrawString(
                Game1.dialogueFont,
                word.wrongCount.ToString(),
                UIUtils.textCenterAlignedPos(word.wrongCount.ToString(),
                new Vector2(xPositionOnScreen + 630, relativeY),
                new Vector2(120, 50)
                ),
                Color.Black);
        }

        public void setWords(List<Word> words)
        {
            this.words = words;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            if(leftButton.containsPoint(x, y))
            {
                LastPage?.Invoke();
                return;
            }
            if(rightButton.containsPoint(x, y))
            {
                NextPage?.Invoke();
                return;
            }
        }
    }
}
