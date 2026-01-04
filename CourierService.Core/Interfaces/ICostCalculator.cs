using CourierService.Core.Models;

namespace CourierService.Core.Interfaces
{
    public interface ICostCalculator
    {
        CostResult Calculate(int baseCost, Package package);
    }
}
