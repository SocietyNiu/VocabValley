using StardewValley;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocabValley.Core.Level;
using VocabValley.Core.Model;
using VocabValley.Core.Points;
using VocabValley.Core.Statistics;
using Microsoft.Xna.Framework;

namespace VocabValley.Utils
{
    internal class DebugManager
    {
        private PointsManager pointsManager;
        public DebugManager(PointsManager pointsManager)
        {
            GameLocation.RegisterTileAction("onDebug", onDebug);
            this.pointsManager = pointsManager;
        }

        private bool onDebug(GameLocation location, string[] args, Farmer player, Point point)
        {
            switch (args[1])
            {
                case "givePoints":
                    pointsManager.changePoints(1000);
                    break;
                default:
                    break;
            }
            return true;
        }
    }
}
