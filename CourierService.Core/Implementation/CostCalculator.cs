using CourierService.Core.Interfaces;
using CourierService.Core.Models;

namespace CourierService.Core.Implementation
{
    public sealed class CostCalculator : ICostCalculator
    {
        private readonly IOfferEngine _offerEngine;

        public CostCalculator(IOfferEngine offerEngine)
        {
            _offerEngine = offerEngine ?? throw new ArgumentNullException(nameof(offerEngine));
        }

        public CostResult Calculate(int baseCost, Package package)
        {
            int deliveryCost = baseCost + (package.WeightKg * 10) + (package.DistanceKm * 5);

            int discount = _offerEngine.GetDiscount(
                deliveryCost,
                package.OfferCode,
                package.WeightKg,
                package.DistanceKm);

            return new CostResult(package.Id, discount, deliveryCost - discount);
        }
    }
}
