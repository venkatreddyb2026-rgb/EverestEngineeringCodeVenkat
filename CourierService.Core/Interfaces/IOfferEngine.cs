namespace CourierService.Core.Interfaces
{
    public interface IOfferEngine
    {
        int GetDiscount(int deliveryCost, string offerCode, int weightKg, int distanceKm);
    }
}
