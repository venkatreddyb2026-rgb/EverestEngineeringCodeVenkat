using CourierService.Core.Interfaces;

namespace CourierService.Core.Implementation
{
    public sealed class OfferOfr003 : IOffer
    {
        public string Code => "OFR003";

        public int CalculateDiscount(int deliveryCost, int weightKg, int distanceKm)
        {
            if (distanceKm >= 50 && distanceKm <= 250 &&
                weightKg >= 10 && weightKg <= 150)
            {
                return CalculatePercentage(deliveryCost, 5);
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
