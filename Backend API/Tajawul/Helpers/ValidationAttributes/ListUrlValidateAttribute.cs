using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ListUrlValidateAttribute : ValidationAttribute
    {
        public int MaxUrlLength { get; set; } = 2048;
        public bool RequireHttpHttps { get; set; } = true;

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // If the list itself is null, it's valid from this attribute's perspective
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // Check if the value is actually an IEnumerable
            if (!(value is IEnumerable list))
            {
                return new ValidationResult($"The {validationContext.DisplayName} field is not a valid list or collection.");
            }

            // Use List<string> for easier handling, check type
            if (!(value is List<string> stringList))
            {
                try
                {
                    // Attempt to cast from IEnumerable<string> if possible
                    stringList = ((IEnumerable<string>)list).ToList();
                }
                catch (Exception)
                {
                    return new ValidationResult($"The {validationContext.DisplayName} field must be a collection of strings.");
                }
            }

            // Iterate and validate each item
            for (int i = 0; i < stringList.Count; i++)
            {
                string? item = stringList[i]; // Get item as potentially null string
                string memberName = $"{validationContext.MemberName}[{i}]"; // Specific item member name

                //Skip validation if item is null/empty
                if (string.IsNullOrWhiteSpace(item))
                {
                    return new ValidationResult($"Please provide a valid URL for item at item {i + 1} in the {validationContext.DisplayName} list.", [memberName]);
                }

                // Check MaxUrlLength
                if (item.Length > MaxUrlLength)
                {
                    return new ValidationResult($"Each URL in the {validationContext.DisplayName} list cannot exceed {MaxUrlLength} characters.", new[] { memberName });
                }

                // Check URL format
                if (!Uri.TryCreate(item, UriKind.Absolute, out var uriResult) ||
                    (RequireHttpHttps && uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
                {
                    string schemeMsg = RequireHttpHttps ? "absolute HTTP or HTTPS " : "absolute ";
                    return new ValidationResult($"Please provide a valid {schemeMsg}URL for item at index {i} in the {validationContext.DisplayName} list.", new[] { memberName });
                }
            }

            // All checks passed
            return ValidationResult.Success;
        }
    }
}