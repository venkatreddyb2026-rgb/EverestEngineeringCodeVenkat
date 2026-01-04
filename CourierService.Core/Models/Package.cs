namespace CourierService.Core.Models
{
    public sealed class Package
    {
        public string Id { get; }
        public int WeightKg { get; }
        public int DistanceKm { get; }
        public string OfferCode { get; }

        public Package(string id, int weightKg, int distanceKm, string offerCode)
        {
            Id = id;
            WeightKg = weightKg;
            DistanceKm = distanceKm;
            OfferCode = offerCode;
        }
    }
}
