using CourierService.Core.Implementation;
using CourierService.Core.Interfaces;
using CourierService.Core.Models;
using System.Text.Json;

try
{
    string input = ReadAllInput();

    var parser = new InputParser();
    var parsed = parser.Parse(input);

    var rules = LoadOfferRules("offers.json");
    IOfferEngine offerEngine = new OfferEngine(rules);
    ICostCalculator costCalculator = new CostCalculator(offerEngine);

    if (parsed.VehicleSpec == null)
    {
        foreach (var p in parsed.Packages)
        {
            var r = costCalculator.Calculate(parsed.BaseCost, p);
            Console.WriteLine($"{r.PackageId} {r.Discount} {r.TotalCost}");
        }
        return;
    }

    IShipmentPlanner planner = new ShipmentPlanner();
    IDeliveryScheduler scheduler = new DeliveryScheduler(costCalculator, planner);

    var results = scheduler.Schedule(parsed.BaseCost, parsed.Packages, parsed.VehicleSpec);

    foreach (var r in results)
        Console.WriteLine($"{r.PackageId} {r.Discount} {r.TotalCost} {r.DeliveryTimeHours:0.00}");
}
catch (InputFormatException ex)
{
    Console.Error.WriteLine(ex.Message);
    Environment.ExitCode = 1;
}
catch (Exception ex)
{
    Console.Error.WriteLine("Unexpected error: " + ex.Message);
    Environment.ExitCode = 1;
}

static string ReadAllInput()
{
    if (Console.IsInputRedirected)
        return Console.In.ReadToEnd();

    var lines = new List<string>();
    string? line;
    while ((line = Console.ReadLine()) != null && line.Length > 0)
        lines.Add(line);

    return string.Join(Environment.NewLine, lines);
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
