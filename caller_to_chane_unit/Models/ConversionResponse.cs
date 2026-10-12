namespace caller_to_chane_unit.Models
{

    public sealed record ConversionResponse(
        double InputValue,
        string FromUnit,
        string ToUnit,
        double Result,
        string Category
    );
}
