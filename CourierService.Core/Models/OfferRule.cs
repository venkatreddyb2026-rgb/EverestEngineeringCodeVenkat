
namespace CourierService.Core.Models
{
    public sealed class OfferRule
    {
        public string Code { get; }
        public int DiscountPercent { get; }
        public int MinWeight { get; }
        public int MaxWeight { get; }
        public int MinDistance { get; }
        public int MaxDistance { get; }

        public OfferRule(
            string code,
            int discountPercent,
            int minWeight,
            int maxWeight,
            int minDistance,
            int maxDistance)
        {
            Code = code;
            DiscountPercent = discountPercent;
            MinWeight = minWeight;
            MaxWeight = maxWeight;
            MinDistance = minDistance;
            MaxDistance = maxDistance;
        }

        public bool IsEligible(int weightKg, int distanceKm)
        {
            return weightKg >= MinWeight && weightKg <= MaxWeight &&
                   distanceKm >= MinDistance && distanceKm <= MaxDistance;
        }
    }
}
