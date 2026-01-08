namespace CourierService.Core.Models
{
    public sealed class ParseResult
    {
        public int BaseCost { get; }
        public IReadOnlyList<Package> Packages { get; }
        public VehicleSpec? VehicleSpec { get; }

        public ParseResult(int baseCost, IReadOnlyList<Package> packages, VehicleSpec? vehicleSpec)
        {
            BaseCost = baseCost;
            Packages = packages;
            VehicleSpec = vehicleSpec;
        }
    }
}
