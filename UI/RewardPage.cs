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
using System.Reflection.Emit;
using xTile;
using VocabValley.Core.Model;

namespace VocabValley.UI
{
    internal class RewardPage : IClickableMenu
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private readonly string cardType = "normal";

        private const int gap = 30;
        private readonly int cardW = 0;
        private readonly int cardH = 0;

        // 用于暴露给外层的更新奖励事件
        public event Action? RewardChosen;

        public List<RewardCard> cards;

        public int rewardChosen = -1;


        public RewardPage(IModHelper helper, IMonitor monitor, string cardType) :
            base(x: (Game1.uiViewport.Width - RewardCardBackground.CARD_RECT[cardType].Width * 3 - gap * 2) / 2,
                 y: (Game1.uiViewport.Height - RewardCardBackground.CARD_RECT[cardType].Height) / 2,
                 width: RewardCardBackground.CARD_RECT[cardType].Width * 3 + gap * 2,
                 height: RewardCardBackground.CARD_RECT[cardType].Height,
                 false)
        {
            this.Helper = helper;
            this.Monitor = monitor;

            this.cardType = cardType;
            cardW = RewardCardBackground.CARD_RECT[cardType].Width;
            cardH = RewardCardBackground.CARD_RECT[cardType].Height;
        }

        public override void draw(SpriteBatch b)
        {
            // 画遮罩层
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            foreach (var card in cards)
            {
                // 绘制卡牌
                int index = cards.IndexOf(card);
                drawCard(b, index, card);
            }

            // 画主体内容
            base.draw(b);

            // 画鼠标
            drawMouse(b);
        }

        public void drawCard(SpriteBatch b, int index, RewardCard card1)
        {
            // 记录卡牌的相对位置
            int reletiveX = xPositionOnScreen + index * (cardW + gap);
            int reletiveY = yPositionOnScreen;

            // 绘制卡牌背景
            b.Draw(RewardCardBackground.TEXTURE, new Vector2(reletiveX, reletiveY),
            sourceRectangle: RewardCardBackground.CARD_RECT[cardType],
            color: Color.White, origin: Vector2.Zero, layerDepth: 0.8f,
            scale: 1.0f, effects: SpriteEffects.None, rotation: 0.0f);

            // 绘制卡牌icon
            float iconX = reletiveX + (cardW - card1.icon.scale * card1.icon.sourceRectangle.Width) / 2;
            float iconY = reletiveY + (250 - card1.icon.scale * card1.icon.sourceRectangle.Height) / 2;
            b.Draw(card1.icon.textureSheet, new Vector2(iconX, iconY),
                sourceRectangle: card1.icon.sourceRectangle,
                color: Color.White, origin: Vector2.Zero, layerDepth: 0.8f,
            scale: card1.icon.scale, effects: SpriteEffects.None, rotation: 0.0f);

            // TODO: 使用UIUtils
            // 计算title文本居中位置
            Vector2 size = Game1.dialogueFont.MeasureString(card1.title);
            Vector2 pos = new Vector2(
                reletiveX + (cardW - size.X) / 2f,
                reletiveY + (400 - size.Y) / 2f
            );

            // 绘制卡牌title

            b.DrawString(
               Game1.dialogueFont,
               card1.title,
               pos,
               Color.White,
               rotation: 0f,
               origin: Vector2.Zero,
               scale: 1.0f,
               effects: SpriteEffects.None,
               layerDepth: 1f
                );

            // 绘制卡牌文本
            b.DrawString(
               Game1.smallFont,
               card1.description,
               new Vector2(reletiveX + 75, reletiveY + 250),
               new Color(252, 189, 106),
               rotation: 0f,
               origin: Vector2.Zero,
               scale: 1.0f,
               effects: SpriteEffects.None,
               layerDepth: 1f
                );
        }
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // 防抖
            // TODO: 测试防抖
            if (rewardChosen != -1)
            {
                return;
            }

            for (int i = 0; i < cards.Count; i++)
            {
                int reletiveX = xPositionOnScreen + i * (cardW + gap);
                int reletiveY = yPositionOnScreen;

                var rect = new Rectangle(reletiveX, reletiveY, cardW, cardH);
                if (rect.Contains(x, y))
                {
                    rewardChosen = i;
                    Game1.playSound("bigSelect");
                    RewardChosen?.Invoke();
                    break;
                }
            }

            base.receiveLeftClick(x, y, playSound);
        }
    }

    static class RewardCardBackground
    {
        public static Rectangle NORMAL_RECT;
        public static Rectangle GRAND_RECT;
        public static Texture2D TEXTURE;
        public static Dictionary<string, Rectangle> CARD_RECT = new()
        {
            ["normal"] = new Rectangle(0, 0, 336, 416),
            ["grand"] = new Rectangle(342, 0, 332, 460)
        };
        public static void init(IModHelper helper)
        {
            TEXTURE = helper.GameContent.Load<Texture2D>("LooseSprites/VocabValley_RewardCard");
        }
    }
}
