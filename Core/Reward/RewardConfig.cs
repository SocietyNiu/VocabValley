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
        public static string[] Diamonds { get; private set; }

        public static string[] Dishes { get; private set; }

        public static void init(IModHelper helper)
        {
            var RewardCardTexture = helper.GameContent.Load<Texture2D>("LooseSprites/VocabValley_RewardCardDetail");
            updateNormalCard(1, helper);

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
            
            Diamonds = new[]{ "60", "62", "64", "66", "68", "70", "72" };

            Dishes = new[] {"194", "195", "196", "197", "198", "199", "200", "201", "202", "203",
                            "204", "205", "206", "207", "208", "209", "210", "211", "212", "213",
                            "214", "215", "216", "218", "219", "220", "221", "222", "223", "224",
                            "225", "226", "227", "228", "229", "230", "231", "232", "233", "234",
                            "235", "236", "237", "238", "239", "240", "241", "242", "243", "244"
            }; 
        }

        public static void updateNormalCard(int p, IModHelper helper)
        {
            Texture2D tex = helper.GameContent.Load<Texture2D>("LooseSprites/VocabValley_RewardCardDetail");
            NormalCards
               = new[]
           {
                new RewardCard(
                    0,
                    new RewardCard.Icon(tex, new Rectangle(0, 0, 16, 16), 3f),
                    "无根之雨",
                    "你的农场里所有农作物\n今天无需浇水",
                    () => RewardImplement.waterCrops()
                ),

                new RewardCard(
                    1,
                    new RewardCard.Icon(tex, new Rectangle( 16,  0, 16, 16), 4f),
                    "友谊之歌",
                    $"与随机一个村民的好感\n度增加{p*75}（上限8心, 1\n心等于250好感度）",
                    () => RewardImplement.grantRandomFriendship(p*75)
                ),
                new RewardCard(
                    2,
                    new RewardCard.Icon(tex, new Rectangle( 32,  0, 16, 16), 4f),
                    "落袋为安",
                    $"获得金币{p*200}",
                    () => RewardImplement.addMoney(200*p)
                ),
                new RewardCard(
                    3,
                    new RewardCard.Icon(tex, new Rectangle( 32,  16, 16, 16), 4f),
                    "田家少闲月",
                    $"随机获得{2 + 2*p}个相同的当\n季生长种子（冬季仅冬\n季种子）",
                    () => RewardImplement.grantRandomSeeds(2 + 2*p)
                ),
                new RewardCard(
                    4,
                    new RewardCard.Icon(tex, new Rectangle( 0,  16, 16, 16), 4f),
                    "逆天改命",
                    $"今天的运气+{p}",
                    () => RewardImplement.grantLuckBuff(helper, p)
                ),
                new RewardCard(
                    5,
                    new RewardCard.Icon(tex, new Rectangle(16, 16, 16, 16), 3f),
                    "学海无涯",
                    "你不要求任何奖励",
                    () => RewardImplement.nothing()
                ),
                /*
                new RewardCard(
                    6,
                    new RewardCard.Icon(RewardCardTexture, new Rectangle(0, 32, 16, 16), 3f),
                    "塔的意志",
                    "获得50知识碎片",
                    () => RewardImplement.nothing()
                ),*/
                new RewardCard(
                    7,
                    new RewardCard.Icon(tex, new Rectangle(16, 32, 16, 16), 3f),
                    "光芒",
                    $"随机获得{p+1}个相同的宝石\n（除五彩碎片）",
                    () => RewardImplement.grandRandomDiamond(p+1)
                ),
                new RewardCard(
                    8,
                    new RewardCard.Icon(tex, new Rectangle(32, 32, 16, 16), 3f),
                    "少食多餐",
                    $"随机获得{p+2}个相同的菜肴",
                    () => RewardImplement.grandDishes(p+2)
                ),
                new RewardCard(
                    9,
                    new RewardCard.Icon(tex, new Rectangle(48, 32, 16, 16), 3f),
                    "风险偏好",
                    $"{p *5}%的几率获得五彩碎片\n{100-p*5}%的几率什么也得不到",
                    () => RewardImplement.grantShard(1, 0.05f * p)
                ),
                new RewardCard(
                    10,
                    new RewardCard.Icon(tex, new Rectangle(0, 48, 16, 16), 3f),
                    "速效救心",
                    $"获得药物各{p}个",
                    () => RewardImplement.grandMedicine(p)
                )
           };
        }
    }
}
