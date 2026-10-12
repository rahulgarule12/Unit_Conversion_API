using caller_to_chane_unit.Models;

namespace caller_to_chane_unit.Repos.Interfaces
{
    public interface IUnitConversionService
    {
        ConversionResponse Convert(ConversionRequest request);

        IReadOnlyCollection<UnitDefinition> GetUnits();
    }
}
