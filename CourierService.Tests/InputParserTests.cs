using CourierService.Core.Implementation;
using CourierService.Core.Models;
using Xunit;

namespace CourierService.Tests
{
    public class InputParserTests
    {
        [Fact]
        public void Throws_when_input_is_empty()
        {
            var parser = new InputParser();
            Assert.Throws<InputFormatException>(() => parser.Parse(""));
        }

        [Fact]
        public void Throws_when_package_count_mismatch()
        {
            var parser = new InputParser();

            string input =
@"100 2
PKG1 5 5 OFR001
"; // missing second package

            Assert.Throws<InputFormatException>(() => parser.Parse(input));
        }

        [Fact]
        public void Throws_when_vehicle_info_incomplete()
        {
            var parser = new InputParser();

            string input =
@"100 1
PKG1 5 5 OFR001
2 70
"; // missing maxLoad

            Assert.Throws<InputFormatException>(() => parser.Parse(input));
        }

        [Fact]
        public void Throws_when_package_weight_exceeds_max_load()
        {
            var parser = new InputParser();

            string input =
@"100 1
PKG1 250 5 OFR001
1 70 200
";

            Assert.Throws<InputFormatException>(() => parser.Parse(input));
        }
    }
}
