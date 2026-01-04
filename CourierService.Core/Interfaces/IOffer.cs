namespace CourierService.Core.Interfaces
{
    public interface IOffer
    {
        string Code { get; }
        int CalculateDiscount(int deliveryCost, int weightKg, int distanceKm);
    }
}
