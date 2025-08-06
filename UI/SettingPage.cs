using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using Microsoft.Xna.Framework;
using VocabValley.Utils;
using VocabValley.Core.Setting;

namespace VocabValley.UI
{
    internal class SettingPage : IClickableMenu
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private const string BGTEXTURE = "LooseSprites/VocabValley_EmptyPage";
        private const string BTTEXTURE = "LooseSprites/VocabValley_Button";

        private readonly Texture2D bgTexture;
        private readonly Texture2D btTexture;

        private readonly ClickableTextureComponent saveButton;
        private List<OptionsCheckbox> boxes = new();

        private SettingManager settingManager;

        public SettingPage(IModHelper helper, IMonitor monitor, 
            SettingManager settingManager):
            base(x: (Game1.uiViewport.Width - 800) / 2,
                 y: (Game1.uiViewport.Height - 600) / 2,
                 width: 800,
                 height: 600,
                 true)
        {
            Helper = helper;
            Monitor = monitor;
            this.settingManager = settingManager;

            bgTexture = helper.GameContent.Load<Texture2D>(BGTEXTURE);
            btTexture = helper.GameContent.Load<Texture2D>(BTTEXTURE);

            saveButton = new ClickableTextureComponent(
                name: "SaveSetting",
                bounds: new Rectangle(xPositionOnScreen + 560, yPositionOnScreen + 450, 240, 120),
                label: "",
                hoverText: "",
                texture: btTexture,
                sourceRect: new Rectangle(0, 0, 240, 120),
                scale: 0.5f);

            boxes.Add(
                new OptionsCheckbox(
                    "暂停塔内时间",
                    877,
                    xPositionOnScreen + 150,
                    yPositionOnScreen + 150
                    )

                );

            boxes[0].isChecked = settingManager.settingState.isPause;
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
                "设置",
                UIUtils.textCenterAlignedPos("设置",
                new Vector2(xPositionOnScreen, yPositionOnScreen+50),
                new Vector2(800, 100)),
                Color.Black);

            b.DrawString(
                Game1.dialogueFont,
                "（学习页面已默认时间暂停，该设置表示塔内其他时间）",
                new Vector2(xPositionOnScreen + 90, yPositionOnScreen + 200),
                Color.Black
                );
            // 画选项复选框
            foreach (var elem in boxes)
            {
                elem.draw(b, 0, 0);
            }

            // 画保存键
            saveButton.draw(b);
            b.DrawString(
                Game1.dialogueFont,
                "保存",
                UIUtils.textCenterAlignedPos("保存",
                new Vector2(saveButton.bounds.X, saveButton.bounds.Y),
                new Vector2(saveButton.bounds.Width, saveButton.bounds.Height),
                "dialogue",
                scale: saveButton.scale),
                Color.Black);

            // 画主体内容
            base.draw(b);

            // 画鼠标
            drawMouse(b);

        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            foreach(var cb in boxes)
            {
                if(cb.bounds.Contains(x, y))
                {
                    cb.receiveLeftClick(x, y);
                    break;
                }
            }

            if(saveButton.bounds.Contains(x, y))
            {
                saveSetting();
                Game1.drawObjectDialogue("设置成功");
                
            }
            base.receiveLeftClick(x, y, playSound);
        }

        private void saveSetting()
        {
            settingManager.settingState.isPause = boxes[0].isChecked;
        }
    }
}
