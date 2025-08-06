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

            cards = RewardConfig.NormalCards
                .Except(cards)
                .OrderBy(_ => rng.Next())
                .Take(3)
                .ToList();
        }
        public void setGrandRewardCard()
        {
            
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
            RewardPage rewardPage = new RewardPage(Helper, Monitor, "normal", shuffleCost);
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
    }
}
