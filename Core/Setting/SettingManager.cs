using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Config;
using VocabValley.Core.Points;
using VocabValley.UI;

namespace VocabValley.Core.Setting
{
    internal class SettingManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public SettingState settingState;
        public int DailyLimit = 5;

        public SettingManager(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;

            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }
        public void onPageCall()
        {
            SettingPage settingPage = new SettingPage(Helper, Monitor, this);
            Game1.activeClickableMenu = settingPage;

        }
        public void setPause(bool isPause)
        {
            if (!Context.IsWorldReady) return;
    
            settingState.isPause = isPause;
        }

        private void OnUpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) return;

            if (settingState!=null && settingState.isPause)
                if(ConstantConfig.TowerMaps.Contains(Game1.currentLocation?.NameOrUniqueName ?? ""))
                {
                    // 如果在塔内 暂停时间
                    Game1.gameTimeInterval = 0;
                }
                
        }

    }
}
