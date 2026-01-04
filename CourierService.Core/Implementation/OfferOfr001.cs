using CourierService.Core.Interfaces;

namespace CourierService.Core.Implementation
{
    public sealed class OfferOfr001 : IOffer
    {
        public string Code => "OFR001";

        public int CalculateDiscount(int deliveryCost, int weightKg, int distanceKm)
        {
            if (distanceKm < 200 &&
                weightKg >= 70 &&
                weightKg <= 200)
            {
                return CalculatePercentage(deliveryCost, 10);
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
