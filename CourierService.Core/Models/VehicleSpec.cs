namespace CourierService.Core.Models
{
    public sealed class VehicleSpec
    {
        public int Count { get; }
        public int SpeedKmPerHour { get; }
        public int MaxLoadKg { get; }

        public VehicleSpec(int count, int speedKmPerHour, int maxLoadKg)
        {
            Count = count;
            SpeedKmPerHour = speedKmPerHour;
            MaxLoadKg = maxLoadKg;
        }
    }
}
