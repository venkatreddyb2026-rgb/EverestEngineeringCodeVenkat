using CourierService.Core.Interfaces;
using CourierService.Core.Models;

namespace CourierService.Core.Implementation
{
    public sealed class OfferEngine : IOfferEngine
    {
        private readonly IDictionary<string, OfferRule> _rules;

        public OfferEngine(IEnumerable<OfferRule> rules)
        {
            _rules = rules.ToDictionary(r => r.Code, r => r, StringComparer.OrdinalIgnoreCase);
        }

        public int GetDiscount(int deliveryCost, string offerCode, int weightKg, int distanceKm)
        {
            if (string.IsNullOrWhiteSpace(offerCode))
                return 0;

            if (!_rules.TryGetValue(offerCode.Trim(), out var rule))
                return 0;

            if (!rule.IsEligible(weightKg, distanceKm))
                return 0;

            return (int)Math.Round(
                deliveryCost * (rule.DiscountPercent / 100m),
                MidpointRounding.AwayFromZero);
        }
    }
}
