using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.ExternalAPIs;

public class ConvertCurrencyInputDto
{
    [Required(ErrorMessage = "FromCurrency is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "FromCurrency must be a 3-letter currency code.")]
    [CurrencyExists(ErrorMessage = "The 'From' currency code is not supported.")] // Apply the custom attribute
    public required string FromCurrency { get; set; }

    [Required(ErrorMessage = "ToCurrency is required.")]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "ToCurrency must be a 3-letter currency code.")]
    [CurrencyExists(ErrorMessage = "The 'To' currency code is not supported.")] // Apply the custom attribute
    public required string ToCurrency { get; set; }

    [Required(ErrorMessage = "Amount is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
}