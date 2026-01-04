using CourierService.Core.Models;

namespace CourierService.Core.Interfaces
{
    public interface IDeliveryScheduler
    {
        IReadOnlyList<DeliveryResult> Schedule(
           int baseCost,
           IReadOnlyList<Package> packages,
           VehicleSpec vehicleSpec);
    }
}
