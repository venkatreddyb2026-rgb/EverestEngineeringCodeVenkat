using CourierService.Core.Models;

namespace CourierService.Core.Interfaces
{
    public interface IShipmentPlanner
    {
        IReadOnlyList<Package> PickShipment(IReadOnlyList<Package> remaining, int maxLoadKg);
    }
}
