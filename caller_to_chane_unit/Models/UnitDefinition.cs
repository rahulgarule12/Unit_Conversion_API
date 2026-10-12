namespace caller_to_chane_unit.Models
{
    public sealed record UnitDefinition(
    string Code,
    string Name,
    string Category,
    double Factor,
    double Offset
);
}
