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
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace VocabValley.UI
{
    internal class SettingVocabPage : IClickableMenu
    {
        private const string BGTEXTURE = "LooseSprites/VocabValley_EmptyPage";
        private const string BTTEXTURE = "LooseSprites/VocabValley_Button";

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private readonly Texture2D bgTexture;
        private readonly Texture2D btTexture;

        public string vocabChosen;
        public string[] files;
        private readonly List<ClickableTextureComponent> chooseButton;
        public event Action<string>? SetVocab;

        public SettingVocabPage(IModHelper helper, IMonitor monitor)
            : base(x: (Game1.uiViewport.Width - 800) / 2,
                 y: (Game1.uiViewport.Height - 600) / 2,
                 width: 800,
                 height: 600,
                 false)
        {
            Helper = helper;
            Monitor = monitor;

            bgTexture = helper.GameContent.Load<Texture2D>(BGTEXTURE);
            btTexture = helper.GameContent.Load<Texture2D>(BTTEXTURE);

            chooseButton = new List<ClickableTextureComponent>();
            
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

            b.DrawString(
                Game1.dialogueFont,
                "当前选择词库：",
                UIUtils.textCenterAlignedPos("当前选择词库：",
                new Vector2(xPositionOnScreen + 150, yPositionOnScreen + 50),
                new Vector2(180, 50)
                ),
                Color.Black);

            b.DrawString(
                Game1.dialogueFont,
                vocabChosen,
                UIUtils.textCenterAlignedPos(vocabChosen,
                new Vector2(xPositionOnScreen + 230, yPositionOnScreen + 50),
                new Vector2(520, 50)
                ),
                Color.Red);

            for(int i=0;i<files.Length;i++)
            {
                int relativeY = i * 50 + 125 + yPositionOnScreen;
                b.DrawString(
                     Game1.dialogueFont,
                     files[i],
                     new Vector2(xPositionOnScreen+150, relativeY),
                     Color.Black
                     );
            }
            drawButton(b);

            base.draw(b);
            // 画鼠标
            drawMouse(b);
        }

        public void drawButton(SpriteBatch b)
        {
            foreach(var button in chooseButton)
            {
                button.draw(b);
                b.DrawString(
                     Game1.smallFont,
                     "选择",
                     UIUtils.textCenterAlignedPos("选择",
                     new Vector2(button.bounds.X, button.bounds.Y),
                     new Vector2(button.bounds.Width, button.bounds.Height),
                     "small",
                     0.35f),
                     Color.Black
                     );
            }
        }

        public void setData(string[] files, string vocabChosen)
        {
            this.files = files;
            this.vocabChosen = vocabChosen;
            for (int i = 0; i < files.Length; i++)
            {
                int relativeY = i * 50 + 125 + yPositionOnScreen;
                chooseButton.Add(
                    new ClickableTextureComponent(
                            name: files[i],
                            bounds: new Rectangle(xPositionOnScreen + 550, relativeY, 240, 120),
                            label: "",
                            hoverText: "",
                            texture: btTexture,
                            sourceRect: new Rectangle(0, 0, 240, 120),
                            scale: 0.35f)
                        );
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach(var button in chooseButton)
            {
                if(button.containsPoint(x, y))
                {
                    SetVocab?.Invoke(button.name);
                }
            }
            base.receiveLeftClick(x, y, playSound);
        }

    }
}
