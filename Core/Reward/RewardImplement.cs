using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buffs;
using StardewValley.GameData.Crops;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VocabValley.UI;
using static StardewValley.Minigames.TargetGame;

namespace VocabValley.Core.Reward
{
    static class RewardImplement
    {
        public static void addMoney(int amount)
        {
            if (!Context.IsWorldReady)
                return; 

            Game1.player.Money += amount;
        }

        public static void waterCrops()
        {
            if (!Context.IsWorldReady)
                return;

            foreach(var farm in Game1.locations.OfType<Farm>())
                foreach(var pair in farm.terrainFeatures.Pairs)
                    if (pair.Value is HoeDirt d) d.state.Value = HoeDirt.watered;
        }


        public static void grantRandomFriendship(int amount)
        {
            if (!Context.IsWorldReady)
                return;

            // 选取可社交的 NPC 列表
            List<NPC> candidates = Utility.getAllVillagers()
                .Where(npc => npc.CanSocialize && !npc.IsInvisible)
                .ToList();

            Random rng = new(Guid.NewGuid().GetHashCode());
            NPC target = candidates[rng.Next(candidates.Count)];

            Game1.player.changeFriendship(amount, target);

            Game1.addHUDMessage(new HUDMessage(
            $"你与 {target.displayName} 的关系提升了 0.5 颗心。", 2));

        }

        public static void grantRandomSeeds(int amount)
        {
            if (!Context.IsWorldReady)
                return;

            string season = Game1.currentSeason.ToLower();
            Random rng = new(Guid.NewGuid().GetHashCode());

            if (RewardConfig.SeasonSeeds.TryGetValue(season, out string[] pool) && pool.Length > 0)
            {
                string seedId = pool[rng.Next(pool.Length)];
                var seed = new StardewValley.Object(seedId, amount);
                if (!Game1.player.addItemToInventoryBool(seed, true))
                    Game1.createItemDebris(seed, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
            }
        }

        public static void grantLuckBuff(IModHelper Helper,int amount)
        {
            if (!Context.IsWorldReady)
                return;

            Buff buff = new Buff(
                id: "VocabValleyLuckBuff",
                displayName: "逆天改命",
                iconTexture: Helper.GameContent.Load<Texture2D>("TileSheets/BuffsIcons"),
                iconSheetIndex: 4,
                duration: Buff.ENDLESS,
                effects: new BuffEffects()
                {
                    LuckLevel = { 3.0f }
                }
            );

            Game1.player.applyBuff(buff);
        }

        public static void nothing()
        {
            Game1.addHUDMessage(new HUDMessage(
            $"你认为学习的动力不来自任何奖励", 2));
            return;
        }

        public static void grantPoints(int amount)
        {
            if (!Context.IsWorldReady)
                return;
        }

        public static void grantRandomDiamond(int amount)
        {
            if (!Context.IsWorldReady)
                return;
            Random rng = new(Guid.NewGuid().GetHashCode());
            string itemId = RewardConfig.Diamonds[rng.Next(RewardConfig.Diamonds.Length)];
            var item = new StardewValley.Object(itemId, amount);
            if (!Game1.player.addItemToInventoryBool(item, true))
                Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
        }

        public static void grantDishes(int amount)
        {
            if (!Context.IsWorldReady)
                return;

            Random rng = new(Guid.NewGuid().GetHashCode());
            string itemId = RewardConfig.Dishes[rng.Next(RewardConfig.Dishes.Length)];
            var item = new StardewValley.Object(itemId, amount);
            if (!Game1.player.addItemToInventoryBool(item, true))
                Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
        }

        public static void grantShard(int amount, float ratio)
        {
            if (!Context.IsWorldReady)
                return;

            Random rng = new(Guid.NewGuid().GetHashCode());
            if(rng.NextDouble() < ratio)
            {
                var item = new StardewValley.Object("74", amount);
                if (!Game1.player.addItemToInventoryBool(item, true))
                    Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
                Game1.showGlobalMessage("你幸运地获得了一块五彩碎片！");
            }
            else
            {
                Game1.showGlobalMessage("什么也没有发生……");
            }
        }

        public static void grantMedicine(int amount)
        {
            if (!Context.IsWorldReady)
                return;

            var item = new StardewValley.Object("349", amount);
            if (!Game1.player.addItemToInventoryBool(item, true))
                Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);

            item = new StardewValley.Object("351", amount);
            if (!Game1.player.addItemToInventoryBool(item, true))
                Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
        }

        public static void grantIridiumSprinklers(int amount = 99)
        {
            if (!Context.IsWorldReady)
                return;

            var item = new StardewValley.Object("645", amount);
            if (!Game1.player.addItemToInventoryBool(item, true))
                Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
        }
        public static void grantAllWarpTotems(int amount = 20)
        {
            if (!Context.IsWorldReady)
                return;

            string[] itemIDs =
            {
                "688", 
                "689", 
                "690", 
                "261", 
                "886"  
            };

            foreach(var itemID in itemIDs)
            {
                var item = new StardewValley.Object(itemID, amount);
                if (!Game1.player.addItemToInventoryBool(item, true))
                    Game1.createItemDebris(item, Game1.player.getStandingPosition(), Game1.player.FacingDirection);
            }
        }
        public static void doubleMoney()
        {
            Game1.player.Money *= 2;
        }
    }
}
