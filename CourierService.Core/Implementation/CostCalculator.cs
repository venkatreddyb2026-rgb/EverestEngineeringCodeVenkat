using CourierService.Core.Interfaces;
using CourierService.Core.Models;

namespace CourierService.Core.Implementation
{
    public sealed class CostCalculator : ICostCalculator
    {
        private readonly IDictionary<string, IOffer> _offers;

        public CostCalculator(IEnumerable<IOffer> offers)
        {
            _offers = offers.ToDictionary(
                o => o.Code,
                o => o,
                StringComparer.OrdinalIgnoreCase);
        }

        public CostResult Calculate(int baseCost, Package package)
        {
            int deliveryCost =
                baseCost +
                (package.WeightKg * 10) +
                (package.DistanceKm * 5);

            int discount = 0;

            if (!string.IsNullOrWhiteSpace(package.OfferCode) &&
                _offers.TryGetValue(package.OfferCode.Trim(), out var offer))
            {
                discount = offer.CalculateDiscount(
                    deliveryCost,
                    package.WeightKg,
                    package.DistanceKm);
            }

            return new CostResult(
                package.Id,
                discount,
                deliveryCost - discount);
        }
    }
}
