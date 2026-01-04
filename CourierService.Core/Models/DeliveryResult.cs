namespace CourierService.Core.Models
{
    public sealed class DeliveryResult
    {
        public string PackageId { get; }
        public int Discount { get; }
        public int TotalCost { get; }
        public double DeliveryTimeHours { get; }

        public DeliveryResult(string packageId, int discount, int totalCost, double deliveryTimeHours)
        {
            PackageId = packageId;
            Discount = discount;
            TotalCost = totalCost;
            DeliveryTimeHours = deliveryTimeHours;
        }
    }
}
