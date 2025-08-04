using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Points;
using VocabValley.UI;

namespace VocabValley.Core.Setting
{
    internal class SettingManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public SettingState settingState;

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
            // 每帧把计时清零，永远无法到达下一个时间
            if (settingState!=null && settingState.isPause)
                Game1.gameTimeInterval = 0;
        }
    }
}
