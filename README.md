# Courier Service – Coding

This project is a command-line application built as part of a courier service coding challenge.  
The goal is to calculate delivery costs, discounts, and delivery times while keeping the code clean, testable, and easy to extend.

I focused on:
- clean separation of concerns
- extensibility 
- testability
- and making it easy for someone else to run and understand the project

## Tech Stack

- .NET (tested with .NET 6+)
- C#
- xUnit for unit testing

No external dependencies or frameworks are required.

## Project Structure

CourierService.sln
  CourierService.Core
      Interfaces // contracts (calculators, planners, schedulers)
      Models // domain models (Package, VehicleSpec, etc.)
      Implementation // business logic

  CourierService.App
    Program.cs // console entry point
    offers.json // offer rules (data-driven)
    
  CourierService.Tests
     *.cs // unit tests

**Core** contains all business logic.  
**App** only handles input/output.  
**Tests** validate behavior.

## Prerequisites
Make sure .NET SDK is installed.

Check using: Open Commannd Prompt and run
dotnet --version

If not installed, download.

**Build the Solution**
From the solution root folder: dotnet build

**Running the Application**
**Run the app:** dotnet run --project CourierService.App

**Cost Calculation**

**Paste the input:**
100 3
PKG1 5 5 OFR001
PKG2 75 125 OFFR0008
PKG3 10 100 OFR003

**Note:** Windows (CMD / PowerShell) → press Ctrl + Z then Enter.

**Expected output format:**
<PACKAGE_ID> <DISCOUNT> <TOTAL_COST>

**Output**
PKG1 0 175
PKG2 0 1475
PKG3 35 665

**Delivery Time Estimation**

**Paste this input:**
100 3
PKG1 5 5 OFR001
PKG2 75 125 OFFR0008
PKG3 10 100 OFR003
2 70 200

**Output format:**
<PACKAGE_ID> <DISCOUNT> <TOTAL_COST> <DELIVERY_TIME>

**Output**
PKG1 0 175 1.78
PKG2 0 1475 0.85
PKG3 35 665 1.42

**Note:** Delivery time is shown in hours, rounded to 2 decimal places.


**Running Tests**
Run - dotnet test

**This will execute:**
cost and discount tests
offer rule tests
shipment planning tests
delivery scheduling tests
input parsing and output formatting tests

**Offer Design (Extensible by Non-Developers)**
Offers are data-driven, not hard-coded.

They are defined in: CourierService.App/offers.json

**Exampe:**
{
  "code": "OFR001",
  "discountPercent": 10,
  "minWeight": 70,
  "maxWeight": 200,
  "minDistance": 0,
  "maxDistance": 199
}
