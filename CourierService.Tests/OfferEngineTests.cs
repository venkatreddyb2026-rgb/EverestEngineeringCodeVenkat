using CourierService.Core.Implementation;
using CourierService.Core.Models;

namespace CourierService.Tests
{
    public class OfferEngineTests
    {
        [Fact]
        public void Returns_zero_when_offer_code_unknown()
        {
            var engine = new OfferEngine(new[]
            {
            new OfferRule("OFR001", 10, 70, 200, 0, 199)
        });

            int discount = engine.GetDiscount(500, "UNKNOWN", 100, 100);

            Assert.Equal(0, discount);
        }

        [Fact]
        public void Applies_discount_when_rule_matches()
        {
            var engine = new OfferEngine(new[]
            {
            new OfferRule("OFR003", 5, 10, 150, 50, 250)
        });

            // deliveryCost 700 => 5% => 35
            int discount = engine.GetDiscount(700, "OFR003", 10, 100);

            Assert.Equal(35, discount);
        }
    }
}
