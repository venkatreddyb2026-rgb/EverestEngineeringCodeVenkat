using CourierService.Core.Interfaces;
using CourierService.Core.Models;

namespace CourierService.Core.Implementation
{
    public sealed class ShipmentPlanner : IShipmentPlanner
    {
        public IReadOnlyList<Package> PickShipment(IReadOnlyList<Package> remaining, int maxLoadKg)
        {
            if (remaining == null || remaining.Count == 0)
                return Array.Empty<Package>();

            var candidates = remaining.Where(p => p.WeightKg <= maxLoadKg).ToList();
            if (candidates.Count == 0)
                return Array.Empty<Package>();

            var dp = new PlanState[maxLoadKg + 1];
            dp[0] = PlanState.Start();

            for (int i = 0; i < candidates.Count; i++)
            {
                var p = candidates[i];

                for (int w = maxLoadKg; w >= p.WeightKg; w--)
                {
                    if (!dp[w - p.WeightKg].HasValue)
                        continue;

                    var prev = dp[w - p.WeightKg];

                    var candidateState = new PlanState(
                        hasValue: true,
                        count: prev.Count + 1,
                        totalWeight: prev.TotalWeight + p.WeightKg,
                        maxDistance: Math.Max(prev.MaxDistance, p.DistanceKm),
                        totalDistance: prev.TotalDistance + p.DistanceKm,
                        prevW: w - p.WeightKg,
                        prevIdx: i
                    );

                    if (!dp[w].HasValue || Better(candidateState, dp[w]))
                        dp[w] = candidateState;
                }
            }

            int bestW = 0;
            bool foundAny = false;
            PlanState best = default;

            for (int w = 0; w <= maxLoadKg; w++)
            {
                if (!dp[w].HasValue) continue;

                if (!foundAny || Better(dp[w], best))
                {
                    best = dp[w];
                    bestW = w;
                    foundAny = true;
                }
            }

            if (!foundAny)
                return Array.Empty<Package>();

            var picked = new List<Package>();
            int curW = bestW;

            while (curW >= 0 && dp[curW].HasValue && dp[curW].PrevIdx >= 0)
            {
                var s = dp[curW];
                picked.Add(candidates[s.PrevIdx]);
                curW = s.PrevW;
            }

            picked.Sort((a, b) =>
            {
                int c = a.DistanceKm.CompareTo(b.DistanceKm);
                if (c != 0) return c;
                return string.Compare(a.Id, b.Id, StringComparison.OrdinalIgnoreCase);
            });

            return picked;
        }

        private static bool Better(PlanState a, PlanState b)
        {
            if (a.Count != b.Count) return a.Count > b.Count;
            if (a.TotalWeight != b.TotalWeight) return a.TotalWeight > b.TotalWeight;

            if (a.MaxDistance != b.MaxDistance) return a.MaxDistance < b.MaxDistance;

            return a.TotalDistance < b.TotalDistance;
        }

        private struct PlanState
        {
            public bool HasValue { get; }
            public int Count { get; }
            public int TotalWeight { get; }
            public int MaxDistance { get; }
            public int TotalDistance { get; }
            public int PrevW { get; }
            public int PrevIdx { get; }

            public PlanState(bool hasValue, int count, int totalWeight, int maxDistance, int totalDistance, int prevW, int prevIdx)
            {
                HasValue = hasValue;
                Count = count;
                TotalWeight = totalWeight;
                MaxDistance = maxDistance;
                TotalDistance = totalDistance;
                PrevW = prevW;
                PrevIdx = prevIdx;
            }

            public static PlanState Start()
            {
                return new PlanState(true, 0, 0, 0, 0, -1, -1);
            }
        }
    }
}
