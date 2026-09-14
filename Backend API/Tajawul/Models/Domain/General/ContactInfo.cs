using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Tajawul.Helpers.ValidationAttributes;


namespace Tajawul.Models.Domain.General
{
    public class ContactInfo : IValidatableObject
    {
        [Required(ErrorMessage = "Contact information type is required.")]
        [EnumDataType(typeof(ContactType), ErrorMessage = "Invalid contact type specified.")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required ContactType Type { get; set; }

        [Required(ErrorMessage = "Contact information value is required.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Contact value must be between 5 and 250 characters.")]
        public required string Value { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!Enum.IsDefined(typeof(ContactType), Type))
            {
                yield return new ValidationResult(
                    $"Invalid contact type value.", // More specific message if needed
                    [nameof(Type)]);
            }

            if (!string.IsNullOrWhiteSpace(Value))
            {
                ValidationAttribute? validator = null;
                switch (Type)
                {
                    case ContactType.Email:
                        validator = new EmailAddressAttribute();
                        break;
                    case ContactType.Phone:
                        validator = new PhoneAttribute();
                        break;
                    case ContactType.Website:
                        validator = new UrlAttribute();
                        break;
                }

                if (validator != null && !validator.IsValid(Value))
                {
                    yield return new ValidationResult(
                        validator.FormatErrorMessage(validationContext.DisplayName ?? nameof(Value)),
                        [nameof(Value)]);
                }
            }
        }
    }

}