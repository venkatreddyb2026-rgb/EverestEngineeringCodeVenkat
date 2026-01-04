using CourierService.Core.Interfaces;

namespace CourierService.Core.Implementation
{
    public sealed class OfferOfr002 : IOffer
    {
        public string Code => "OFR002";

        public int CalculateDiscount(int deliveryCost, int weightKg, int distanceKm)
        {
            if (distanceKm >= 50 &&
                distanceKm <= 150 &&
                weightKg >= 100 &&
                weightKg <= 250)
            {
                return CalculatePercentage(deliveryCost, 7);
            }

            return 0;
        }

        private static int CalculatePercentage(int amount, int percent)
        {
            return (int)Math.Round(
                amount * (percent / 100m),
                MidpointRounding.AwayFromZero);
        }
    }
}
