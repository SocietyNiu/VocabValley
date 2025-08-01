using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VocabValley.Utils
{
    internal class CountDown
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;
        private readonly int targetMinutes;
        private readonly int targetSeconds;

        public int remainSeconds;

        public event Action? updateCountDownEvent;


        public CountDown(IModHelper helper, IMonitor monitor, int minutes, int seconds)
        {
            Helper = helper;
            Monitor = monitor;
            targetMinutes = minutes;
            targetSeconds = seconds;

            remainSeconds = transfer2seconds(targetMinutes, targetSeconds);
        }
        public CountDown(IModHelper helper, IMonitor monitor,int seconds)
        {
            Helper = helper;
            Monitor = monitor;
            (targetMinutes, targetSeconds) = transfer2minutes(seconds);
            remainSeconds = seconds;
        }

        public void start()
        {
            Helper.Events.GameLoop.OneSecondUpdateTicked += onOneSecond;

        }
        public void stop()
        {
            Helper.Events.GameLoop.OneSecondUpdateTicked -= onOneSecond;
        }
        private void onOneSecond(object? sender, OneSecondUpdateTickedEventArgs e)
        {
            if(remainSeconds <= 0)
            {
                stop();
                return;
            }
            remainSeconds--;
            updateCountDownEvent?.Invoke();
        }
        public string getCountDown()
        {
            return transfer2string(remainSeconds);
        }
        static int transfer2seconds(int min, int seconds)
        {
            return min * 60 + seconds;
        }
        static (int, int) transfer2minutes(int seconds)
        {
            return (seconds / 60, seconds % 60);
        }
        static string transfer2string(int seconds)
        {
            var (min, sec) = transfer2minutes(seconds);
            return $"{min:D2}:{sec:D2}";
        }
    }
}
