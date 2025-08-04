using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace VocabValley.Core.Points
{
    internal class PointsManager
    {
        private readonly IModHelper Helper;
        private readonly IMonitor Monitor;

        public int points;

        private readonly HashSet<string> _allowedMaps = new()
        {
            "BabelTower",
            "BabelTowerNormalLevel",
            "BabelTowerStairLevel",
            "BabelTowerRewardLevel",
            "BabelTowerBossLevel"
        };

        public PointsManager(IModHelper helper, IMonitor monitor)
        {
            Helper = helper;
            Monitor = monitor;

            helper.Events.Display.RenderedHud += OnRenderedHud;
        }

        private void OnRenderedHud(object? sender, StardewModdingAPI.Events.RenderedHudEventArgs e)
        {
            if (!Context.IsWorldReady || Game1.eventUp)
                return;

            if (!_allowedMaps.Contains(Game1.currentLocation.Name))
                return;
            SpriteBatch sb = e.SpriteBatch;

            Texture2D icon = Helper.GameContent.Load<Texture2D>("LooseSprites/VocabValley_Points");

            sb.Draw(
                texture: icon,
                position: new Vector2(16, 16), 
                sourceRectangle: new Rectangle(0, 0, 32, 32),
                color: Color.White, origin: Vector2.Zero, layerDepth: 0.8f,
               scale: 1f, effects: SpriteEffects.None, rotation: 0.0f);

            sb.DrawString(Game1.smallFont, points.ToString(), new Vector2(48, 16), Color.White);
        }

        public bool changePoints(int amount)
        {
            if(this.points + amount < 0)
            {
                Game1.drawObjectDialogue("你没有足够的知识碎片。");
                return false;
            }
            this.points += amount;
            return true;
        }

        
    }
}
