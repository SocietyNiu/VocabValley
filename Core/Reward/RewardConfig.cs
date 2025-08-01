using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using VocabValley.Core.Model;

namespace VocabValley.Core.Reward
{
    internal static class RewardConfig
    {
        public static RewardCard[] NormalCards { get; private set; } = Array.Empty<RewardCard>();
        public static RewardCard[] GrandCards { get; private set; } = Array.Empty<RewardCard>();
        public static RewardCard[] CheatCards { get; private set; } = Array.Empty<RewardCard>();

        public static Dictionary<string, string[]> SeasonSeeds { get; private set; }

        public static void init(IModHelper helper)
        {
            var RewardCardTexture = helper.GameContent.Load<Texture2D>("LooseSprites/VocabValley_RewardCardDetail");
            NormalCards = new[]
            {
                new RewardCard(
                    0,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle(0, 0, 16, 16), 3f),
                    "无根之雨",
                    "你的农场里所有农作物\n今天无需浇水",
                    () => RewardImplement.waterCrops()
                ),

                new RewardCard(
                    1,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle( 16,  0, 16, 16), 4f),
                    "友谊之歌",
                    "与随机一个村民的好感\n度+0.5颗心（上限8心）",
                    () => RewardImplement.grantRandomFriendship(125)
                ),
                new RewardCard(
                    2,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle( 32,  0, 16, 16), 4f),
                    "落袋为安",
                    "获得金币200",
                    () => RewardImplement.addMoney(200)
                ),
                new RewardCard(
                    3,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle( 32,  16, 16, 16), 4f),
                    "田家少闲月",
                    "随机获得6个相同的当\n季生长种子（冬季仅冬\n季种子）",
                    () => RewardImplement.grantRandomSeeds(6)
                ),
                new RewardCard(
                    4,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle( 0,  16, 16, 16), 4f),
                    "逆天改命",
                    "今天的运气+3",
                    () => RewardImplement.grantLuckBuff(helper, 3)
                ),
                new RewardCard(
                    5,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle(16, 16, 16, 16), 3f),
                    "学海无涯",
                    "你不要求任何奖励",
                    () => RewardImplement.nothing()
                )
            };
            GrandCards = new[]
            {
                new RewardCard(
                    0,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle( 48,  0, 16, 16), 4f),
                    "天气之子",
                    "你的所有土地不用再浇水\n（包括温室和姜岛）",
                    () =>{}
                ),
                new RewardCard(
                    1,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle( 0,  16, 16, 16), 4f),
                    "幸运儿",
                    "你将一直保持好运",
                    () =>{}
                ),
                new RewardCard(
                    2,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle( 32,  0, 16, 16), 4f),
                    "议价权",
                    "你售出的所有商品售价\n提高5%",
                    () =>{}
                )
            };
            CheatCards = new[]
            {
                new RewardCard(
                    0,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle(16, 16, 16, 16), 3f),
                    "持久作战",
                    "Boss战时间更长，但\n需要解决的单词也更多",
                    () =>{}
                ),
                new RewardCard(
                    0,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle(16, 16, 16, 16), 3f),
                    "记忆洪流",
                    "普通战词汇上限提升，\nBoss触发率同步上升。",
                    () =>{}
                ),
                new RewardCard(
                    0,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle(16, 16, 16, 16), 3f),
                    "错词缓冲",
                    "普通战错答单词不会在\n本轮再次出现，但是仍然\n会在以后出现",
                    () =>{}
                ),
            };

            SeasonSeeds = new()
            {
                ["spring"] =  new[]
                {
                    "472", "473", "474", "475", "476", "477", "478", "273", "427", "429", "745"
                },
                ["summer"] = new[]
                {  
                    "479", "480", "481", "482", "483","484", "485", "486", "487","434", "302", "453", "455"  
                },
                ["fall"] = new[]
                {
                    "488", "489", "490", "491", "492"," 493", "494","299","301","425","483"
                },
                ["winter"] = new[]
                {
                    "498"
                }

            };
        }
    }
}
