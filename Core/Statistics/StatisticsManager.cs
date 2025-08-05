using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VocabValley.Core.Model;
using VocabValley.UI;

namespace VocabValley.Core.Statistics
{
    internal class StatisticsManager
    {

        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public StatisticsState statisticsState;
        public WordsManager wordsManager;

        // 2) 本次打开菜单的开始时间；null = 当前没在统计
        private DateTime? _sessionStartUtc;
        public StatisticsManager(IModHelper helper, IMonitor monitor, WordsManager wordsManager) 
        {
            helper.Events.Display.MenuChanged += onMenuChanged;
            this.Helper = helper;
            this.Monitor = monitor;
            this.wordsManager = wordsManager;
        }

        public void onShowPageCall()
        {
            StatisticsPage sp = new StatisticsPage(Helper, Monitor, statisticsState, wordsManager);
            Game1.activeClickableMenu = sp;
        }


        private readonly HashSet<Type> trackedMenus = new() 
        {
            typeof(WordLearningPage), 
            typeof(BossPage) 
        };
        private void onMenuChanged(object? sender, MenuChangedEventArgs e)
        {
            bool newIsTracked = e.NewMenu is not null && trackedMenus.Contains(e.NewMenu.GetType());
            bool oldIsTracked = e.OldMenu is not null && trackedMenus.Contains(e.OldMenu.GetType());

            // 记录进入UI时间
            if (newIsTracked && _sessionStartUtc is null)
                _sessionStartUtc = DateTime.UtcNow;

            // 记录离开时间
            if (oldIsTracked && !newIsTracked && _sessionStartUtc is not null)
            {
                statisticsState.totalSeconds += (DateTime.UtcNow - _sessionStartUtc.Value).TotalSeconds;
                _sessionStartUtc = null;
            }
        }
    }
}
