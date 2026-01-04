using CourierService.Core.Interfaces;
using CourierService.Core.Models;
using CourierService.Core.Implementation;
using System.Globalization;

string input;

if (Console.IsInputRedirected)
{
    input = Console.In.ReadToEnd();
}
else
{
    var lines = new List<string>();
    string? line;
    while ((line = Console.ReadLine()) != null && line.Length > 0)
        lines.Add(line);

    input = string.Join(Environment.NewLine, lines);
}

var tokens = input.Split(null as char[], StringSplitOptions.RemoveEmptyEntries);

int index = 0;

int baseCost = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
int packageCount = int.Parse(tokens[index++], CultureInfo.InvariantCulture);

var packages = new List<Package>(packageCount);

for (int i = 0; i < packageCount; i++)
{
    string id = tokens[index++];
    int weight = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
    int distance = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
    string offer = tokens[index++];

    packages.Add(new Package(id, weight, distance, offer));
}

var costCalculator = new CostCalculator(new IOffer[]
{
    new OfferOfr001(),
    new OfferOfr002(),
    new OfferOfr003()
});

if (index >= tokens.Length)
{
    foreach (var pkg in packages)
    {
        var result = costCalculator.Calculate(baseCost, pkg);
        Console.WriteLine($"{result.PackageId} {result.Discount} {result.TotalCost}");
    }

    return;
}

int vehicleCount = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
int maxSpeed = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
int maxLoad = int.Parse(tokens[index++], CultureInfo.InvariantCulture);

var vehicleSpec = new VehicleSpec(vehicleCount, maxSpeed, maxLoad);

IShipmentPlanner planner = new ShipmentPlanner();
IDeliveryScheduler scheduler = new DeliveryScheduler(costCalculator, planner);

var deliveryResults = scheduler.Schedule(baseCost, packages, vehicleSpec);

foreach (var r in deliveryResults)
{
    Console.WriteLine($"{r.PackageId} {r.Discount} {r.TotalCost} {r.DeliveryTimeHours:0.00}");
}
