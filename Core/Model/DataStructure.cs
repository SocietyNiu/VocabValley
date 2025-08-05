using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile;

namespace VocabValley.Core.Model
{
    internal class Word
    {
        public int ID { get; set; }
        public string text { get; set; }
        public string translation { get; set; }
        public bool isLearned { get; set; }
        public int reviewCount { get; set; }
        public int wrongCount { get; set; }
        // Quiz选项的ID
        public List<int> options { get; set; } 

        public Word(int id, string Text, string Translation)
        {
            ID = id;
            text = Text;
            translation = Translation;
        }

        public Word(string Text, string Translation)
        {
            text = Text;
            translation = Translation;
        }
        public Word() {}
    }

    internal class RewardCard
    {
        public class Icon
        {
            public Texture2D textureSheet { get; set; }
            public Rectangle sourceRectangle { get; set; }
            public float scale { get; set; }

            public Icon(Texture2D textureSheet, Rectangle sourceRectangle, float scale)
            {
                this.textureSheet = textureSheet;
                this.sourceRectangle = sourceRectangle;
                this.scale = scale;
            }
        }

        public int ID { get; set; }
        public Icon icon { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public Action executeReward { get; set; }
        public RewardCard(int id, Icon icon, string title, string description, Action executeReward)
        {
            ID = id;
            this.icon = icon;
            this.title = title;
            this.description = description;
            this.executeReward = executeReward;
        }
    }

    public record StatisticsState
    {
        public double totalSeconds = 0;
        public int normalLevelCount = 0;
        public int BossLevelCount = 0;
    }
}
