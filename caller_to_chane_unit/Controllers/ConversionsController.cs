using caller_to_chane_unit.Models;
using Microsoft.AspNetCore.Mvc;
using caller_to_chane_unit.Repos.Services;
using caller_to_chane_unit.Repos.Interfaces;

namespace caller_to_chane_unit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConversionsController : ControllerBase
    {
        private readonly IUnitConversionService _service;

        public ConversionsController(IUnitConversionService service)
        {
            _service = service;
        }

        [HttpGet("convert")]
        public IActionResult Convert(
    [FromQuery] double value,
    [FromQuery] MeasurementCategory category,
    [FromQuery] MeasurementUnit fromUnit,
    [FromQuery] MeasurementUnit toUnit)
        {
            var fromCode = ToUnitCode(fromUnit);
            var toCode = ToUnitCode(toUnit);

            var request = new ConversionRequest
            {
                Value = value,
                FromUnit = fromCode,
                ToUnit = toCode
            };

            try
            {
                var result = _service.Convert(request);

                if (!string.Equals(
                    result.Category,
                    category.ToString(),
                    StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new
                    {
                        error = "Selected units do not match the category."
                    });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
        private static string ToUnitCode(MeasurementUnit unit)
        {
            return unit switch
            {
                MeasurementUnit.Meters => "m",
                MeasurementUnit.Kilometers => "km",
                MeasurementUnit.Centimeters => "cm",
                MeasurementUnit.Feet => "ft",
                MeasurementUnit.Inches => "in",
                MeasurementUnit.Miles => "mi",
                MeasurementUnit.Celsius => "c",
                MeasurementUnit.Fahrenheit => "f",
                MeasurementUnit.Kelvin => "k",
                MeasurementUnit.Kilograms => "kg",
                MeasurementUnit.Grams => "g",
                MeasurementUnit.Milligrams => "mg",
                MeasurementUnit.Pounds => "lb",
                MeasurementUnit.Ounces => "oz",
                _ => throw new ArgumentOutOfRangeException(nameof(unit))
            };
        }
    }
}
