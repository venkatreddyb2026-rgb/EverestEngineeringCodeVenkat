using CourierService.Core.Models;
using System.Globalization;
using System.Text;

namespace CourierService.Core.Implementation
{
    public sealed class OutputFormatter
    {
        public string FormatCostResults(IReadOnlyList<CostResult> results)
        {
            var sb = new StringBuilder();

            foreach (var r in results)
            {
                sb.Append(r.PackageId)
                  .Append(' ')
                  .Append(r.Discount.ToString(CultureInfo.InvariantCulture))
                  .Append(' ')
                  .Append(r.TotalCost.ToString(CultureInfo.InvariantCulture))
                  .AppendLine();
            }

            return sb.ToString();
        }

        public string FormatDeliveryResults(IReadOnlyList<DeliveryResult> results)
        {
            var sb = new StringBuilder();

            foreach (var r in results)
            {
                sb.Append(r.PackageId)
                  .Append(' ')
                  .Append(r.Discount.ToString(CultureInfo.InvariantCulture))
                  .Append(' ')
                  .Append(r.TotalCost.ToString(CultureInfo.InvariantCulture))
                  .Append(' ')
                  .Append(r.DeliveryTimeHours.ToString("0.00", CultureInfo.InvariantCulture))
                  .AppendLine();
            }

            return sb.ToString();
        }
    }
}
