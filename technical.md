# Technical Design – Courier Service Application

## Overview

This application solves a courier service problem in two stages:

1. Calculate delivery cost and discount for each package
2. Calculate delivery time by grouping packages into optimal shipments using available vehicles

The app runs as a console program, but the core logic is kept separate so it is easy to test, understand, and extend.

---

## Project Structure

The solution is split into three projects:

- **CourierService.Core**  
  Contains all business logic: cost calculation, offers, shipment planning, delivery scheduling, input parsing, and output formatting.

- **CourierService.App**  
  The console entry point. It reads input, wires everything together, and prints the final output.

- **CourierService.Tests**  
  Unit tests covering business rules, input parsing, output formatting, and edge cases.

This separation keeps responsibilities clear and avoids mixing console code (UI) with business logic.

---

## Application Flow

1. Read all input from standard input
2. Parse and validate the input
3. Load offer rules from `offers.json`
4. Calculate cost and discount for each package
5. If vehicle information is present:
   - Plan shipments
   - Assign shipments to vehicles
   - Calculate delivery times
6. Format and print the output

---

## Input Parsing

Input parsing is handled by a dedicated class called `InputParser`.

### Why this exists

Console input is fragile. Users can enter:
- missing values
- non-numeric values
- incomplete package rows
- invalid vehicle data

Keeping parsing logic separate makes the code easier to test and keeps `Program.cs` clean.

### What InputParser does

- Reads tokens from the raw input
- Validates base cost and package count
- Validates each package row
- Optionally parses vehicle information
- Detects extra or missing input
- Throws `InputFormatException` with a clear message when input is invalid

---

## Cost Calculation

For each package, the delivery cost is calculated as:

```
baseCost + (weight × 10) + (distance × 5)
```

Any applicable discount is applied after calculating the delivery cost.

### CostCalculator

`CostCalculator`:
- Knows how to compute delivery cost
- Delegates discount logic to an offer engine
- Returns a simple result object

This keeps pricing logic stable even when offers change.

---

## Offer Design (Extensible)

### Problem with hard-coded offers

Hard-coding offers in C# means:
- Every new offer requires a code change
- Business teams depend on developers for simple updates

### Solution: data-driven offers

Offers are defined in `offers.json` instead of code.

Example:
```json
{
  "code": "OFR001",
  "discountPercent": 10,
  "minWeight": 70,
  "maxWeight": 200,
  "minDistance": 0,
  "maxDistance": 199
}
```

### OfferEngine

`OfferEngine`:
- Loads all offer rules
- Finds a rule by offer code
- Checks if a package qualifies
- Calculates discount percentage

This allows new offers to be added without changing code.

---

## Shipment Planning

Shipment planning is handled by `ShipmentPlanner`.

### Rules

When selecting a shipment:

1. Maximize number of packages
2. If tied, choose the shipment with higher total weight
3. If still tied, choose the shipment that delivers earlier
4. Final tie-breakers ensure deterministic behavior

This logic is explicit and easy to follow, which makes tests reliable.

---

## Delivery Scheduling

Delivery scheduling is handled by `DeliveryScheduler`.

### Responsibilities

- Track vehicle availability
- Assign the next shipment to the earliest available vehicle
- Calculate delivery time per package
- Calculate vehicle return time

### Delivery time

```
deliveryTime = vehicleStartTime + (distance / speed)
```

### Vehicle return time

```
returnTime = startTime + 2 × (farthestDistance / speed)
```

---

## Output Formatting

Output formatting is handled by `OutputFormatter`.

### Why formatting is separate

Separating formatting:
- Keeps console code simple
- Makes output easy to test
- Ensures consistent spacing and rounding

The formatter returns strings, which are printed by `Program.cs`.

---

## Error Handling

### Expected errors

- Invalid input format
- Missing or extra values
- Invalid numbers
- Package weight exceeding vehicle capacity

These result in clear error messages and a non-zero exit code.

### Unexpected errors

Unexpected issues are caught at the top level and reported with a generic message.

---

## Testing Strategy

The test suite covers:

- Cost calculation
- Offer rule evaluation
- Shipment selection logic
- Delivery scheduling
- Input parsing
- Output formatting

---

## Final Notes

The goal of this solution is clarity and correctness. A new developer should be able to read the code, understand the flow, and make changes without breaking unrelated parts of the system.

