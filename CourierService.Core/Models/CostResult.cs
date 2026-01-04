namespace CourierService.Core.Models
{
    public sealed class CostResult
    {
        public string PackageId { get; }
        public int Discount { get; }
        public int TotalCost { get; }

        public CostResult(string packageId, int discount, int totalCost)
        {
            PackageId = packageId;
            Discount = discount;
            TotalCost = totalCost;
        }
    }
}
