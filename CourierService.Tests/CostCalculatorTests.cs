using CourierService.Core.Implementation;
using CourierService.Core.Models;

namespace CourierService.Tests
{
    public class CostCalculatorTests
    {
        [Fact]
        public void Calculates_total_cost_with_discount()
        {
            var engine = new OfferEngine(new[]
            {
            new OfferRule("OFR003", 5, 10, 150, 50, 250)
        });

            var calc = new CostCalculator(engine);

            var pkg = new Package("PKG3", 10, 100, "OFR003");

            var result = calc.Calculate(100, pkg);

            // deliveryCost = 100 + 10*10 + 100*5 = 700
            // discount = 35
            // total = 665
            Assert.Equal("PKG3", result.PackageId);
            Assert.Equal(35, result.Discount);
            Assert.Equal(665, result.TotalCost);
        }
    }
}