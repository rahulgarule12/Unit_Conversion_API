# Unit Converter API

An ASP.NET Core Web API for converting numeric values between units of measurement. The initial version supports **Length**, **Temperature**, and **Weight/Mass** conversions and uses Swagger UI for API exploration and testing.

## 1. Features

- Convert a numeric value from one unit to another.
- Support three measurement categories:
  - **Length:** meters, kilometers, centimeters, feet, inches, miles
  - **Temperature:** Celsius, Fahrenheit, Kelvin
  - **Weight/Mass:** kilograms, grams, milligrams, pounds, ounces
- Centralize unit definitions in a unit registry.
- Use a shared conversion service instead of implementing every possible unit pair separately.
- Validate unknown units and incompatible categories.
- Expose API documentation through Swagger UI.
- Keep the design open to storing unit definitions in a database in a future version.

## 2. Technology

- ASP.NET Core Web API
- C#
- Swashbuckle.AspNetCore (Swagger UI)
- Built-in dependency injection

Use the .NET version targeted by the project. The examples below assume a modern ASP.NET Core project.

## 3. Project structure

A suggested structure is:

```text
UnitConverterApi/
├── Controllers/
│   └── ConversionsController.cs
├── Models/
│   ├── ConversionRequest.cs
│   ├── ConversionResponse.cs
│   ├── UnitDefinition.cs
│   ├── MeasurementCategory.cs
│   ├── LengthUnit.cs
│   ├── WeightUnit.cs
│   └── TemperatureUnit.cs
├── Services/
│   ├── IUnitConversionService.cs
│   ├── UnitConversionService.cs
│   └── UnitRegistry.cs
├── Program.cs
└── README.md
```

The exact files depend on the implementation selected. For example, category-specific enums are optional if the API uses string unit codes instead.

## 4. How conversion works

Length and mass units use a factor to convert to a base unit. Temperature uses a factor and an offset.

```text
baseValue = inputValue * sourceFactor + sourceOffset
result    = (baseValue - targetOffset) / targetFactor
```

Base units:

- Length: meter
- Temperature: Celsius
- Weight/Mass: kilogram

Example definitions:

| Unit | Code | Category | Factor | Offset |
|---|---|---|---:|---:|
| Meter | `m` | Length | 1 | 0 |
| Foot | `ft` | Length | 0.3048 | 0 |
| Celsius | `c` | Temperature | 1 | 0 |
| Fahrenheit | `f` | Temperature | 5/9 | -160/9 |
| Kelvin | `k` | Temperature | 1 | -273.15 |
| Kilogram | `kg` | Mass | 1 | 0 |
| Pound | `lb` | Mass | 0.45359237 | 0 |

Temperature conversions also need validation to prevent values below absolute zero.

## 5. Setup

### Prerequisites

- .NET SDK compatible with the project
- A code editor such as Visual Studio or Visual Studio Code

### Create the project (if needed)

```bash
dotnet new webapi --use-controllers -n UnitConverterApi
cd UnitConverterApi
```

### Install Swagger UI

From the project directory, run:

```bash
dotnet add package Swashbuckle.AspNetCore
```

### Configure `Program.cs`

Register controllers, Swagger, and the conversion dependencies. Adjust the namespace to match your project.

```csharp
using UnitConverterApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<UnitRegistry>();
builder.Services.AddScoped<IUnitConversionService, UnitConversionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

If the project currently uses `AddOpenApi()` and `MapOpenApi()`, those are the built-in OpenAPI document features. For the Swashbuckle UI workflow shown here, use `AddSwaggerGen()`, `UseSwagger()`, and `UseSwaggerUI()` instead. Avoid registering duplicate OpenAPI configurations unnecessarily.

## 6. Run the API

```bash
dotnet restore
dotnet build
dotnet run
```

Open the Swagger URL using the address and port printed by the application, for example:

```text
https://localhost:<port>/swagger
```

Do not assume the example port is the same as your local port.

## 7. API endpoint

The single conversion endpoint used in the current workflow is:

```http
GET /api/Conversions/convert?value=10&fromUnit=m&toUnit=ft
```

Example parameters:

- `value`: numeric value to convert
- `fromUnit`: source unit code
- `toUnit`: target unit code

Example response:

```json
{
  "inputValue": 10,
  "fromUnit": "m",
  "toUnit": "ft",
  "result": 32.80839895013123,
  "category": "Length"
}
```

Other examples:

### Celsius to Fahrenheit

```http
GET /api/Conversions/convert?value=100&fromUnit=c&toUnit=f
```

Expected result: `212`.

### Kilograms to pounds

```http
GET /api/Conversions/convert?value=10&fromUnit=kg&toUnit=lb
```

Expected result: approximately `22.0462`.

### Invalid category combination

A request such as meters to kilograms should return HTTP `400 Bad Request`, because both units must belong to the same category.

## 8. Swagger dropdown behavior

### Current enum-based dropdowns

C# enums can provide dropdown-style choices for unit parameters in Swagger. Separate enums for `LengthUnit`, `WeightUnit`, and `TemperatureUnit` can ensure that each category-specific endpoint only offers valid units for that category.

### Desired single-workflow behavior

The requested UX is:

1. Select a category: Length, Weight, or Temperature.
2. Show only units for that category in `From Unit`.
3. Show only units for that category in `To Unit`.
4. Enter a value and execute the conversion.

**Important:** Default Swagger UI does not dynamically filter one query-parameter dropdown based on another parameter in the same operation. Adding a category enum alone does not provide this behavior. A custom Swagger UI JavaScript/plugin enhancement is required to keep a single endpoint and dynamically update both unit dropdowns.

Until that customization is implemented, use one of these alternatives:

- Separate endpoints with category-specific enums, which provide static category-specific unit choices; or
- A dedicated frontend form (for example, React) with a category dropdown that dynamically updates the source and target unit options.

Regardless of the UI, the API must validate category compatibility on the server.

## 9. Error handling and validation

The API should return a clear `400 Bad Request` response for cases such as:

- Unknown source unit
- Unknown target unit
- Source and target units belong to different categories
- Non-finite numeric values
- Temperature below absolute zero

Example error:

```json
{
  "error": "Source and target units must belong to the same category."
}
```

For a production implementation, consider using consistent ASP.NET Core `ProblemDetails` responses.

## 10. Dependency injection lifetimes

The example uses:

- `Singleton` for `UnitRegistry`, because the registry is read-only after construction.
- `Scoped` for `IUnitConversionService`, so the service is resolved within each request scope.

If unit definitions later become editable at runtime, the registry and caching strategy must be redesigned to handle updates safely.

## 11. Testing checklist

- [ ] `10 m` to `ft` returns approximately `32.8084`.
- [ ] `100 c` to `f` returns `212`.
- [ ] `10 kg` to `lb` returns approximately `22.0462`.
- [ ] Unknown unit codes return HTTP 400.
- [ ] Length-to-mass conversion is rejected.
- [ ] Temperature below absolute zero is rejected.
- [ ] Swagger UI opens successfully.
- [ ] Category-dependent filtering is tested separately if custom Swagger UI behavior is implemented.

## 12. Future improvements

1. Store units and categories in a database instead of hardcoding them.
2. Add an endpoint that returns units for a specified category.
3. Add unit and integration tests.
4. Use a consistent error response format.
5. Add rounding/precision policies if required by clients.
6. Add API versioning and logging.
7. Implement the custom Swagger UI behavior or provide a dedicated frontend for dependent dropdowns.

## Summary

The API uses a centralized registry and shared conversion service to avoid hardcoding each possible conversion pair. Swagger UI allows developers to test the API. Static category-specific enum dropdowns are possible with separate endpoints; dynamically filtering source and target units in one Swagger operation requires an additional UI customization.
