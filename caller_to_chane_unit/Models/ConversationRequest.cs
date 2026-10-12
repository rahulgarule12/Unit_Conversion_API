using System.ComponentModel.DataAnnotations;

namespace caller_to_chane_unit.Models
{
    
    public sealed class ConversionRequest
    {
        [Required]
        public string FromUnit { get; set; } = string.Empty;

        [Required]
        public string ToUnit { get; set; } = string.Empty;

        public double Value { get; set; }
    }
}
