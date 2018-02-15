using GenesisVision.DataModel;
using GenesisVision.DataModel.Enums;
using GenesisVision.DataModel.Models;
using GenesisVision.Tournament.Core.Models;
using GenesisVision.Tournament.Core.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenesisVision.Tournament.Core.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly ApplicationDbContext context;
        private readonly IMemoryCache memoryCache;
        private const string ratingKey = "rating";

        public StatisticService(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            this.context = context;
            this.memoryCache = memoryCache;
        }

        public void RecalculateChart(TradeAccounts account, int pointsCount = 30, ChartType type = ChartType.ByProfit)
        {
            var result = new List<decimal> {0};

            var startBalance = account.StartBalance;
            var profits = account.Trades
                                 .OrderBy(x => x.Date)
                                 .Select(x => x.Profit)
                                 .ToList();

            var statistic = new List<decimal>();
            var balances = new List<decimal>();
            statistic.Add(0m);
            balances.Add(startBalance);
            for (var i = 0; i < profits.Count; i++)
            {
                statistic.Add((balances[i] + profits[i]) / startBalance * 100m - 100m);
                balances.Add(balances[i] + profits[i]);
            }

            var list = new List<List<decimal>>();
            var step = statistic.Count <= pointsCount
                ? 1
                : statistic.Count % pointsCount >= pointsCount / 3
                    ? statistic.Count / pointsCount + 1
                    : statistic.Count / pointsCount;

            var count = 0;
            do
            {
                list.Add(statistic.Skip(count).Take(step).ToList());
                count += step;
            } while (count < statistic.Count);

            if (!list.Any() || !list.First().Any())
                return;

            switch (type)
            {
                case ChartType.ByProfit:
                    result = list
                        .Select(x => Math.Round(x.Average(y => y), 2))
                        .ToList();
                    break;
            }

            if (profits.Count > pointsCount)
            {
                result.Add(account.TotalProfitInPercent);
            }

            context.RemoveRange(account.Charts.Where(x => x.Type == type));

            var index = 0;
            foreach (var chart in result)
            {
                context.Add(new Charts
                            {
                                Id = Guid.NewGuid(),
                                Type = type,
                                Index = index,
                                TradeAccountId = account.Id,
                                Value = chart
                            });
                index++;
            }

            context.SaveChanges();
        }

        public void RecalculatePlaces()
        {
            InvokeOperations.InvokeOperation(() =>
            {
                var accounts = context.TradeAccounts
                                      .OrderByDescending(x => x.TotalProfit)
                                      .Where(x => x.ParticipantId != null)
                                      .Select(x => new
                                                   {
                                                       x.ParticipantId,
                                                       x.OrdersCount,
                                                       x.TotalProfit
                                                   })
                                      .ToList();

                var ratingList = new List<Guid>();
                var withoutOrders = accounts.Where(x => x.OrdersCount == 0).ToList();
                var lastPlace = accounts.Count - withoutOrders.Count + 1;
                var place = 1;
                foreach (var account in accounts)
                {
                    if (account.OrdersCount >= 1)
                    {
                        ratingList.Add(account.ParticipantId.Value);
                        memoryCache.Set(account.ParticipantId, place);
                        place++;
                    }
                    else
                    {
                        memoryCache.Set(account.ParticipantId, lastPlace);
                    }
                }
                ratingList.AddRange(withoutOrders.Select(x => x.ParticipantId.Value));
                memoryCache.Set(ratingKey, ratingList);
            });
        }

        public int GetParticipantPlace(Guid participantId)
        {
            return TryGetObjectFromCache(participantId, out int value)
                ? value
                : -1;
        }

        public List<Guid> GetParticipantsByPlace(int? skip, int? take)
        {
            return TryGetObjectFromCache(ratingKey, out List<Guid> value)
                ? value.Skip(skip ?? 0)
                       .Take(take ?? int.MaxValue)
                       .ToList()
                : new List<Guid>();
        }

        private bool TryGetObjectFromCache<T>(object key, out T value)
        {
            if (memoryCache.TryGetValue(key, out value))
                return true;

            RecalculatePlaces();

            if (memoryCache.TryGetValue(key, out value))
                return true;

            return false;
        }
    }
}
