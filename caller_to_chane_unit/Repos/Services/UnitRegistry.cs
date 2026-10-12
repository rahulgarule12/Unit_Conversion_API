using caller_to_chane_unit.Models;

namespace caller_to_chane_unit.Repos.Services
{
    public class UnitRegistry
    {
        private readonly Dictionary<string, UnitDefinition> _units =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Length: base unit = meter
            ["m"] = new("m", "Meter", "Length", 1, 0),
            ["km"] = new("km", "Kilometer", "Length", 1000, 0),
            ["cm"] = new("cm", "Centimeter", "Length", 0.01, 0),
            ["ft"] = new("ft", "Foot", "Length", 0.3048, 0),
            ["in"] = new("in", "Inch", "Length", 0.0254, 0),
            ["mi"] = new("mi", "Mile", "Length", 1609.344, 0),

            // Temperature: base unit = Celsius
            ["c"] = new("c", "Celsius", "Temperature", 1, 0),
            ["f"] = new("f", "Fahrenheit", "Temperature",
                        5.0 / 9.0, -32.0 * 5.0 / 9.0),
            ["k"] = new("k", "Kelvin", "Temperature", 1, -273.15),

            // Mass: base unit = kilogram
            ["kg"] = new("kg", "Kilogram", "Mass", 1, 0),
            ["g"] = new("g", "Gram", "Mass", 0.001, 0),
            ["mg"] = new("mg", "Milligram", "Mass", 0.000001, 0),
            ["lb"] = new("lb", "Pound", "Mass", 0.45359237, 0),
            ["oz"] = new("oz", "Ounce", "Mass", 0.028349523125, 0)
        };

        public bool TryGetUnit(
            string code,
            out UnitDefinition unit)
        {
            return _units.TryGetValue(code.Trim(), out unit!);
        }

        public IReadOnlyCollection<UnitDefinition> GetAll()
        {
            return _units.Values.ToArray();
        }
    }
}
