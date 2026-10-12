using caller_to_chane_unit.Models;
using caller_to_chane_unit.Repos.Interfaces;

namespace caller_to_chane_unit.Repos.Services;

public sealed class UnitConversionService(
    UnitRegistry registry) : IUnitConversionService
{
    public ConversionResponse Convert(ConversionRequest request)
    {
        if (!double.IsFinite(request.Value))
        {
            throw new ArgumentException(
                "Value must be a finite number.");
        }

        if (!registry.TryGetUnit(request.FromUnit, out var from))
        {
            throw new ArgumentException(
                $"Unknown source unit: {request.FromUnit}");
        }

        if (!registry.TryGetUnit(request.ToUnit, out var to))
        {
            throw new ArgumentException(
                $"Unknown target unit: {request.ToUnit}");
        }

        if (!string.Equals(
                from.Category,
                to.Category,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Source and target units must belong to the same category.");
        }

        // Convert source value to the category's base unit.
        double baseValue =
            request.Value * from.Factor + from.Offset;

        // Absolute zero is the minimum possible temperature.
        if (from.Category == "Temperature" &&
            baseValue < -273.15)
        {
            throw new ArgumentException(
                "Temperature cannot be below absolute zero.");
        }

        // Convert the base value into the target unit.
        double result =
            (baseValue - to.Offset) / to.Factor;

        return new ConversionResponse(
            request.Value,
            from.Code,
            to.Code,
            result,
            from.Category);
    }

    public IReadOnlyCollection<UnitDefinition> GetUnits()
    {
        return registry.GetAll();
    }
}

