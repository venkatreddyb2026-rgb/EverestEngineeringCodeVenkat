using CourierService.Core.Implementation;
using CourierService.Core.Models;

namespace CourierService.Tests
{
    public class OutputFormatterTests
    {
        [Fact]
        public void Formats_cost_results_exactly()
        {
            var formatter = new OutputFormatter();

            var results = new[]
            {
                new CostResult("PKG1", 0, 175),
                new CostResult("PKG2", 0, 1475),
                new CostResult("PKG3", 35, 665)
            };

            var text = formatter.FormatCostResults(results);

            var expected =
                "PKG1 0 175" + Environment.NewLine +
                "PKG2 0 1475" + Environment.NewLine +
                "PKG3 35 665" + Environment.NewLine;

            Assert.Equal(expected, text);
        }

        [Fact]
        public void Formats_delivery_results_with_two_decimals()
        {
            var formatter = new OutputFormatter();

            var results = new[]
            {
                new DeliveryResult("PKG1", 0, 175, 1.777),
                new DeliveryResult("PKG2", 0, 1475, 0.85),
                new DeliveryResult("PKG3", 35, 665, 1.4)
            };

            var text = formatter.FormatDeliveryResults(results);

            var expected =
                "PKG1 0 175 1.78" + Environment.NewLine +
                "PKG2 0 1475 0.85" + Environment.NewLine +
                "PKG3 35 665 1.40" + Environment.NewLine;

            Assert.Equal(expected, text);
        }
    }
}
