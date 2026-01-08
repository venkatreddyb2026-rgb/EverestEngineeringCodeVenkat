using CourierService.Core.Interfaces;
using CourierService.Core.Models;

namespace CourierService.Core.Implementation
{
    public sealed class DeliveryScheduler : IDeliveryScheduler
    {
        private readonly ICostCalculator _costCalculator;
        private readonly IShipmentPlanner _shipmentPlanner;

        public DeliveryScheduler(ICostCalculator costCalculator, IShipmentPlanner shipmentPlanner)
        {
            _costCalculator = costCalculator;
            _shipmentPlanner = shipmentPlanner;
        }

        public IReadOnlyList<DeliveryResult> Schedule(int baseCost, IReadOnlyList<Package> packages, VehicleSpec vehicleSpec)
        {
            if (packages == null) throw new ArgumentNullException(nameof(packages));
            if (vehicleSpec == null) throw new ArgumentNullException(nameof(vehicleSpec));
            if (vehicleSpec.Count <= 0) throw new ArgumentException("Vehicle count must be > 0.");
            if (vehicleSpec.SpeedKmPerHour <= 0) throw new ArgumentException("Vehicle speed must be > 0.");
            if (vehicleSpec.MaxLoadKg <= 0) throw new ArgumentException("Vehicle max load must be > 0.");

            var costById = new Dictionary<string, CostResult>(StringComparer.OrdinalIgnoreCase);
            foreach (var p in packages)
                costById[p.Id] = _costCalculator.Calculate(baseCost, p);

            var deliveryTimeById = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);

            var remaining = new List<Package>(packages);

            var pq = new PriorityQueue<VehicleState, VehiclePriority>();
            for (int i = 1; i <= vehicleSpec.Count; i++)
                pq.Enqueue(new VehicleState(i, 0.0), new VehiclePriority(0.0, i));

            while (remaining.Count > 0)
            {
                var vehicle = pq.Dequeue();
                double startTime = vehicle.AvailableAt;

                var shipment = _shipmentPlanner.PickShipment(remaining, vehicleSpec.MaxLoadKg).ToList();
                if (shipment.Count == 0)
                {
                    var fallback = remaining
                        .Where(p => p.WeightKg <= vehicleSpec.MaxLoadKg)
                        .OrderBy(p => p.WeightKg)
                        .ThenBy(p => p.DistanceKm)
                        .FirstOrDefault();

                    if (fallback == null)
                        throw new InvalidOperationException("No package can fit in the vehicle capacity.");

                    shipment.Add(fallback);
                }

                int farthest = 0;

                foreach (var p in shipment)
                {
                    farthest = Math.Max(farthest, p.DistanceKm);
                    double deliveredAt = startTime + (p.DistanceKm / (double)vehicleSpec.SpeedKmPerHour);
                    deliveryTimeById[p.Id] = deliveredAt;
                }

                double tripTime = farthest / (double)vehicleSpec.SpeedKmPerHour;
                double availableAgain = startTime + (2.0 * tripTime);

                foreach (var p in shipment)
                    remaining.Remove(p);

                pq.Enqueue(new VehicleState(vehicle.VehicleId, availableAgain),
                           new VehiclePriority(availableAgain, vehicle.VehicleId));
            }

            // Output in original input order
            var results = new List<DeliveryResult>(packages.Count);
            foreach (var p in packages)
            {
                var cost = costById[p.Id];
                double t = deliveryTimeById[p.Id];
                results.Add(new DeliveryResult(cost.PackageId, cost.Discount, cost.TotalCost, Round2(t)));
            }

            return results;
        }

        private static double Round2(double value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        private sealed class VehicleState
        {
            public int VehicleId { get; }
            public double AvailableAt { get; }

            public VehicleState(int vehicleId, double availableAt)
            {
                VehicleId = vehicleId;
                AvailableAt = availableAt;
            }
        }

        private readonly struct VehiclePriority : IComparable<VehiclePriority>
        {
            public double AvailableAt { get; }
            public int VehicleId { get; }

            public VehiclePriority(double availableAt, int vehicleId)
            {
                AvailableAt = availableAt;
                VehicleId = vehicleId;
            }

            public int CompareTo(VehiclePriority other)
            {
                int c = AvailableAt.CompareTo(other.AvailableAt);
                if (c != 0) return c;
                return VehicleId.CompareTo(other.VehicleId);
            }
        }
    }
}
