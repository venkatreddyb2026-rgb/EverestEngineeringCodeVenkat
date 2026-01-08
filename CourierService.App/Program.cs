using CourierService.Core.Implementation;
using CourierService.Core.Interfaces;
using CourierService.Core.Models;
using System.Globalization;
using System.Text.Json;

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

if (tokens.Length < 2)
{
    Console.Error.WriteLine("Invalid input: missing base cost or package count.");
    Environment.ExitCode = 1;
    return;
}

int index = 0;

int baseCost = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
int packageCount = int.Parse(tokens[index++], CultureInfo.InvariantCulture);

var packages = new List<Package>(packageCount);

for (int i = 0; i < packageCount; i++)
{
    if (index + 3 >= tokens.Length)
    {
        Console.Error.WriteLine("Invalid input: incomplete package details.");
        Environment.ExitCode = 1;
        return;
    }

    string id = tokens[index++];
    int weight = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
    int distance = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
    string offer = tokens[index++];

    packages.Add(new Package(id, weight, distance, offer));
}

// Load offers.json
var rules = LoadOfferRules("offers.json");

IOfferEngine offerEngine = new OfferEngine(rules);
ICostCalculator costCalculator = new CostCalculator(offerEngine);

// Part 1 (no vehicle info)
if (index >= tokens.Length)
{
    foreach (var pkg in packages)
    {
        var result = costCalculator.Calculate(baseCost, pkg);
        Console.WriteLine($"{result.PackageId} {result.Discount} {result.TotalCost}");
    }

    return;
}

// Part 2 (vehicle info exists)
if (index + 2 >= tokens.Length)
{
    Console.Error.WriteLine("Invalid input: vehicle info is incomplete.");
    Environment.ExitCode = 1;
    return;
}

int vehicleCount = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
int speed = int.Parse(tokens[index++], CultureInfo.InvariantCulture);
int maxLoad = int.Parse(tokens[index++], CultureInfo.InvariantCulture);

var vehicleSpec = new VehicleSpec(vehicleCount, speed, maxLoad);

IShipmentPlanner planner = new ShipmentPlanner();
IDeliveryScheduler scheduler = new DeliveryScheduler(costCalculator, planner);

var deliveryResults = scheduler.Schedule(baseCost, packages, vehicleSpec);

foreach (var r in deliveryResults)
{
    Console.WriteLine($"{r.PackageId} {r.Discount} {r.TotalCost} {r.DeliveryTimeHours:0.00}");
}

static List<OfferRule> LoadOfferRules(string fileName)
{
    if (!File.Exists(fileName))
        throw new FileNotFoundException($"Offer configuration file not found: {fileName}");

    string json = File.ReadAllText(fileName);

    var raw = JsonSerializer.Deserialize<List<OfferRuleDto>>(
        json,
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

    if (raw == null || raw.Count == 0)
        throw new InvalidOperationException("offers.json is empty or invalid.");

    var rules = new List<OfferRule>(raw.Count);

    foreach (var r in raw)
    {
        if (string.IsNullOrWhiteSpace(r.Code))
            continue;

        rules.Add(new OfferRule(
            r.Code.Trim(),
            r.DiscountPercent,
            r.MinWeight,
            r.MaxWeight,
            r.MinDistance,
            r.MaxDistance));
    }

    return rules;
}

sealed class OfferRuleDto
{
    public string Code { get; set; } = "";
    public int DiscountPercent { get; set; }
    public int MinWeight { get; set; }
    public int MaxWeight { get; set; }
    public int MinDistance { get; set; }
    public int MaxDistance { get; set; }
}
