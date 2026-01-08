using CourierService.Core.Models;
using System.Globalization;

namespace CourierService.Core.Implementation
{
    public sealed class InputParser
    {
        public ParseResult Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new InputFormatException("Input is empty. Please provide base cost and package details.");

            var tokens = input.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            int index = 0;

            int baseCost = ReadInt(tokens, ref index, "base delivery cost");
            if (baseCost < 0)
                throw new InputFormatException("Base delivery cost must be 0 or greater.");

            int packageCount = ReadInt(tokens, ref index, "number of packages");
            if (packageCount <= 0)
                throw new InputFormatException("Number of packages must be greater than 0.");

            var packages = new List<Package>(packageCount);

            for (int i = 1; i <= packageCount; i++)
            {
                string id = ReadString(tokens, ref index, $"package[{i}] id");

                int weight = ReadInt(tokens, ref index, $"package[{i}] weight");
                if (weight <= 0)
                    throw new InputFormatException($"Package '{id}' weight must be greater than 0.");

                int distance = ReadInt(tokens, ref index, $"package[{i}] distance");
                if (distance <= 0)
                    throw new InputFormatException($"Package '{id}' distance must be greater than 0.");

                string offer = ReadString(tokens, ref index, $"package[{i}] offer code");

                packages.Add(new Package(id, weight, distance, offer));
            }

            VehicleSpec? vehicleSpec = null;

            // We expect exactly 3 numbers.
            if (index < tokens.Length)
            {
                int vehicleCount = ReadInt(tokens, ref index, "number of vehicles");
                int speed = ReadInt(tokens, ref index, "vehicle speed");
                int maxLoad = ReadInt(tokens, ref index, "vehicle max load");

                if (vehicleCount <= 0)
                    throw new InputFormatException("Number of vehicles must be greater than 0.");
                if (speed <= 0)
                    throw new InputFormatException("Vehicle speed must be greater than 0.");
                if (maxLoad <= 0)
                    throw new InputFormatException("Vehicle max load must be greater than 0.");

                // If any extra junk tokens exist, that’s invalid input
                if (index < tokens.Length)
                    throw new InputFormatException("Invalid input: extra values found after vehicle configuration.");

                // Every package can fit in a vehicle
                foreach (var p in packages)
                {
                    if (p.WeightKg > maxLoad)
                        throw new InputFormatException($"Package '{p.Id}' weight ({p.WeightKg}) exceeds max load ({maxLoad}).");
                }

                vehicleSpec = new VehicleSpec(vehicleCount, speed, maxLoad);
            }

            return new ParseResult(baseCost, packages, vehicleSpec);
        }

        private static int ReadInt(string[] tokens, ref int index, string name)
        {
            if (index >= tokens.Length)
                throw new InputFormatException($"Invalid input: missing {name}.");

            string raw = tokens[index++];

            if (!int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                throw new InputFormatException($"Invalid input: '{raw}' is not a valid integer for {name}.");

            return value;
        }

        private static string ReadString(string[] tokens, ref int index, string name)
        {
            if (index >= tokens.Length)
                throw new InputFormatException($"Invalid input: missing {name}.");

            string value = tokens[index++].Trim();

            if (string.IsNullOrWhiteSpace(value))
                throw new InputFormatException($"Invalid input: {name} cannot be empty.");

            return value;
        }
    }
}
