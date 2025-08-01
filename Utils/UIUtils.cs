using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewValley;

namespace VocabValley.Utils
{
    static class UIUtils
    {
        public static Vector2 textCenterAlignedPos(string content, Vector2 pos, Vector2 size)
        {
            if (string.IsNullOrEmpty(content))
                return new Vector2();
            // 计算文本的宽度和高度
            Vector2 textSize = Game1.dialogueFont.MeasureString(content);

            // 计算文本的中心位置
            // size.X是文本宽度
            float x = pos.X + (size.X - textSize.X) / 2f;
            float y = pos.Y + (size.Y - textSize.Y) / 2f;
            return new Vector2(x, y);
        }
        public static Vector2 textCenterAlignedPos(string content, Vector2 pos, Vector2 size, string type, float scale = 1.0f)
        {
            Vector2 textSize = new Vector2();
            if (type == "dialogue")
                textSize = Game1.dialogueFont.MeasureString(content);
            else if(type == "small")
                textSize = Game1.smallFont.MeasureString(content);

            // size.X是文本宽度
            float x = pos.X + (size.X * scale - textSize.X) / 2f;
            float y = pos.Y + (size.Y * scale - textSize.Y) / 2f;
            return new Vector2(x, y);
        }
    }
}
