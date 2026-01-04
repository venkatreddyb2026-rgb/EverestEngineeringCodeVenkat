using CourierService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourierService.Core.Interfaces
{
    public interface IShipmentPlanner
    {
        IReadOnlyList<Package> PickShipment(IReadOnlyList<Package> remaining, int maxLoadKg);
    }
}
