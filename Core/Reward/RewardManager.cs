using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.Core.Points;
using VocabValley.UI;

namespace VocabValley.Core.Reward
{
    internal class RewardManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        private PointsManager pointsManager;

        public List<RewardCard> cards;
        public string type = "normal";
        public int shuffleCost = 50;
        public bool isRewarded = false;

        public int premiumLevel = 1;
        private int premiumPrice = 200;


        public RewardManager(IModHelper helper, IMonitor monitor, PointsManager pointsManager)
        {
            Helper = helper;
            Monitor = monitor;
            this.pointsManager = pointsManager;

            cards = new List<RewardCard>();
        }
        public void init()
        {
            RewardConfig.init(Helper);
            RewardCardBackground.init(Helper);
        }

        public void reset()
        {
            // 没有取过
            isRewarded = false;

            // 重置奖励卡片
            shuffleAndSetCard();
        }
        public void setRandomNormalRewardCard()
        {
            Random rng = Game1.random;
            type = "normal";

            cards = RewardConfig.NormalCards
                .Except(cards)
                .OrderBy(_ => rng.Next())
                .Take(3)
                .ToList();
        }
        public void setGrandRewardCard()
        {
            type = "grand";
            cards = RewardConfig.GrandCards.ToList();
        }
        public void setCheatRewardCard()
        {
        }
        public void shuffleAndSetCard()
        {
            setRandomNormalRewardCard();
        }

        public void onRewardPageCall()
        {
            if(isRewarded == true)
            {
                Game1.drawObjectDialogue("你已选择过奖励");
                return;
            }
            RewardPage rewardPage = new RewardPage(Helper, Monitor, type, shuffleCost);
            rewardPage.cards = cards;

            rewardPage.RewardChosen += () =>
            {
                isRewarded = true;
                Game1.exitActiveMenu();
                cards[rewardPage.rewardChosen].executeReward?.Invoke();
                Game1.drawObjectDialogue("你选择了"+ cards[rewardPage.rewardChosen].title);
            };

            rewardPage.Shuffle += () =>
            {
                // TODO: 调整shuffle价格
                if(pointsManager.changePoints(-shuffleCost))
                {
                    shuffleAndSetCard();
                    rewardPage.cards = cards;
                }
            };

            Game1.activeClickableMenu = rewardPage;
        }

        public void updateNormalReward()
        {
            RewardConfig.updateNormalCard(premiumLevel, Helper);
        }

        public void updatePremiumLevel()
        {
            if (premiumLevel >= 5)
            {
                Game1.drawObjectDialogue("您已达到最高奖励等级");
                return;
            }

            if(pointsManager.points < premiumPrice)
            {
                Game1.drawObjectDialogue($"你现在的奖励等级为:{premiumLevel},升级需要{premiumPrice}知识碎片");
                return;
            } else
            {
                string Question = $"你现在的奖励等级为:{premiumLevel},你想用{premiumPrice}知识碎片升级吗?";
                Response[] opts = {
                    new("YES", "是的"),
                    new("NO",  "我再想想")
                };
                Game1.currentLocation.createQuestionDialogue(
                    Question,
                    opts,
                    (farmer, ans) =>
                    {
                        switch (ans)
                        {
                            case "YES":
                                if (pointsManager.changePoints(-premiumPrice))
                                {
                                    premiumLevel++;
                                    updateNormalReward();
                                    Game1.drawObjectDialogue($"升级成功，你现在的等级为：{premiumLevel}");
                                }
                                break;
                            case "NO":
                                break;
                        }
                    });
            }
        }
    }
}
