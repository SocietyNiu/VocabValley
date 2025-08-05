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

        private void onMenuChanged(object? sender, MenuChangedEventArgs e) 
        { 
            if(settingState is null ||  !settingState.isPause)
            {
                return;
            }

            // 如果是学习页面则停止计时
            if(e.NewMenu is WordLearningPage || e.NewMenu is BossPage)
            {
                Helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            } else if(e.OldMenu is WordLearningPage || e.OldMenu is BossPage)
            {
                Helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked;
            }
        }


    }
}
