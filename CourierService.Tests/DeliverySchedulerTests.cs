using CourierService.Core.Implementation;
using CourierService.Core.Interfaces;
using CourierService.Core.Models;

namespace CourierService.Tests
{
    public class DeliverySchedulerTests
    {
        [Fact]
        public void Computes_delivery_times_with_vehicle_return_time()
        {
            var costCalc = new FakeCostCalculator();
            var planner = new ShipmentPlanner();
            var scheduler = new DeliveryScheduler(costCalc, planner);

            var packages = new List<Package>
            {
                new Package("P1", 50, 30, "NA"),
                new Package("P2", 100, 125, "NA"),
                new Package("P3", 75, 100, "NA")
            };

            var vehicles = new VehicleSpec(count: 1, speedKmPerHour: 50, maxLoadKg: 200);

            var results = scheduler.Schedule(baseCost: 100, packages: packages, vehicleSpec: vehicles);

            AssertResult(results, "P3", expectedTime: 2.00);
            AssertResult(results, "P2", expectedTime: 2.50);
            AssertResult(results, "P1", expectedTime: 5.60);
        }

        private static void AssertResult(IReadOnlyList<DeliveryResult> results, string id, double expectedTime)
        {
            var r = Find(results, id);
            Assert.InRange(r.DeliveryTimeHours, expectedTime - 0.01, expectedTime + 0.01);
        }

        private static DeliveryResult Find(IReadOnlyList<DeliveryResult> results, string id)
        {
            foreach (var r in results)
                if (r.PackageId == id) return r;

            throw new KeyNotFoundException(id);
        }

        private sealed class FakeCostCalculator : ICostCalculator
        {
            public CostResult Calculate(int baseCost, Package package)
            {
                return new CostResult(package.Id, discount: 0, totalCost: 999);
            }
        }
    }
}
