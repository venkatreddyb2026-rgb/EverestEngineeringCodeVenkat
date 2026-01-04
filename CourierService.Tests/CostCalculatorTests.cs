using CourierService.Core.Implementation;
using CourierService.Core.Interfaces;
using CourierService.Core.Models;
using Xunit;

namespace CourierService.Tests
{
    public class CostCalculatorTests
    {
        private static CostCalculator CreateCalculator()
        {
            return new CostCalculator(new IOffer[]
            {
            new OfferOfr001(),
            new OfferOfr002(),
            new OfferOfr003()
            });
        }

        [Fact]
        public void Offer001_applies_ten_percent_discount_when_eligible()
        {
            var offer = new OfferOfr001();
            int discount = offer.CalculateDiscount(700, 100, 150);

            Assert.Equal(70, discount);
        }

        [Fact]
        public void Offer002_does_not_apply_discount_when_weight_is_low()
        {
            var offer = new OfferOfr002();
            int discount = offer.CalculateDiscount(500, 90, 100);

            Assert.Equal(0, discount);
        }

        [Fact]
        public void Invalid_offer_results_in_zero_discount()
        {
            var calculator = CreateCalculator();
            var package = new Package("PKG1", 75, 125, "INVALID");

            var result = calculator.Calculate(100, package);

            Assert.Equal(0, result.Discount);
            Assert.Equal(100 + (75 * 10) + (125 * 5), result.TotalCost);
        }

        [Fact]
        public void Offer003_applies_five_percent_discount()
        {
            var calculator = CreateCalculator();
            var package = new Package("PKG3", 10, 100, "OFR003");

            var result = calculator.Calculate(100, package);

            Assert.Equal(35, result.Discount);
            Assert.Equal(665, result.TotalCost);
        }
    }
}
