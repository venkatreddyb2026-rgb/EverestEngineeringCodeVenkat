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

            List<Package>? best = null;

            foreach (var combo in GenerateCombos(candidates))
            {
                int totalWeight = combo.Sum(p => p.WeightKg);
                if (totalWeight > maxLoadKg)
                    continue;

                if (best == null || Better(combo, best))
                    best = combo;
            }

            if (best == null)
                return Array.Empty<Package>();

            // deterministic order within shipment
            best.Sort((a, b) =>
            {
                int c = a.DistanceKm.CompareTo(b.DistanceKm);
                if (c != 0) return c;
                return string.Compare(a.Id, b.Id, StringComparison.OrdinalIgnoreCase);
            });

            return best;
        }

        private static IEnumerable<List<Package>> GenerateCombos(List<Package> items)
        {
            int n = items.Count;

            for (int i = 0; i < n; i++)
                yield return new List<Package> { items[i] };

            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    yield return new List<Package> { items[i], items[j] };

            for (int i = 0; i < n; i++)
                for (int j = i + 1; j < n; j++)
                    for (int k = j + 1; k < n; k++)
                        yield return new List<Package> { items[i], items[j], items[k] };
        }

        private static bool Better(List<Package> a, List<Package> b)
        {
            // 1) maximize package count
            if (a.Count != b.Count) return a.Count > b.Count;

            // 2) if tie, maximize total weight
            int aWeight = a.Sum(p => p.WeightKg);
            int bWeight = b.Sum(p => p.WeightKg);
            if (aWeight != bWeight) return aWeight > bWeight;

            // 3) if tie, prefer smaller farthest distance (earlier delivery/return)
            int aMaxDist = a.Max(p => p.DistanceKm);
            int bMaxDist = b.Max(p => p.DistanceKm);
            if (aMaxDist != bMaxDist) return aMaxDist < bMaxDist;

            // 4) deterministic tie-breakers
            int aTotalDist = a.Sum(p => p.DistanceKm);
            int bTotalDist = b.Sum(p => p.DistanceKm);
            if (aTotalDist != bTotalDist) return aTotalDist < bTotalDist;

            string aKey = string.Join("|", a.OrderBy(x => x.Id).Select(x => x.Id));
            string bKey = string.Join("|", b.OrderBy(x => x.Id).Select(x => x.Id));
            return string.Compare(aKey, bKey, StringComparison.OrdinalIgnoreCase) < 0;
        }
    }
}
