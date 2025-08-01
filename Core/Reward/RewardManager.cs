using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.UI;

namespace VocabValley.Core.Reward
{
    internal class RewardManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public List<RewardCard> cards;
        public string type = "normal";
        public RewardManager(IModHelper helper, IMonitor monitor, string type)
        {
            Helper = helper;
            Monitor = monitor;
            this.type = type;

            cards = new List<RewardCard>();
            RewardConfig.init(helper);
            RewardCardBackground.init(helper);
        }

        public void setRandomNormalRewardCard()
        {
            Random rng = Game1.random;

            cards = RewardConfig.NormalCards
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

        public void onRewardPageCall()
        {
            RewardPage rewardPage = new RewardPage(Helper, Monitor, "normal");

            cards = new List<RewardCard>();
            setRandomNormalRewardCard();

            rewardPage.cards = cards;

            rewardPage.RewardChosen += () =>
            {
                Game1.exitActiveMenu();
                cards[rewardPage.rewardChosen].executeReward?.Invoke();
                Game1.drawObjectDialogue("你选择了"+ cards[rewardPage.rewardChosen].title);
            };

            Game1.activeClickableMenu = rewardPage;
        }
    }
}
