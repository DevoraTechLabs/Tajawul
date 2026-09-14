using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes 
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class EachAllowedDestinationTypeAttribute : ValidationAttribute
    {
        // --- NO NEED TO REPEAT STATIC MEMBERS OR LOADING LOGIC ---
        // We will rely on the static members and methods already defined
        // in DestinationTypeExistsAttribute. They are shared because they are static.

        public EachAllowedDestinationTypeAttribute() : base("One or more destination types are not supported.")
        {
        }

        public EachAllowedDestinationTypeAttribute(string errorMessage) : base(errorMessage)
        {
        }

        // Override the IsValid method to perform the validation on a collection
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // --- Access the SHARED static members from DestinationTypeExistsAttribute ---
            var supportedTypes = DestinationTypeExistsAttribute._supportedDestinationTypes;

            // Ensure destination types are loaded before validation runs
            if (supportedTypes == null)
            {
                return new ValidationResult("Destination Type validation list is not available. Server configuration error.");
            }

            // The value is expected to be a collection of strings
            var destinationTypeList = value as IEnumerable<string>;

            // If the list is null or empty, it's considered valid (or let [Required] handle null)
            // An empty list trivially satisfies the condition "all elements are valid".
            if (destinationTypeList == null || !destinationTypeList.Any())
            {
                return ValidationResult.Success;
            }

            var invalidTypes = new List<string>();

            foreach (var destinationType in destinationTypeList)
            {
                // Ignore null or whitespace entries within the list
                if (string.IsNullOrWhiteSpace(destinationType))
                {
                    continue;
                }

                if (!supportedTypes.Contains(destinationType, StringComparer.OrdinalIgnoreCase))
                {
                    invalidTypes.Add(destinationType); // Collect invalid types
                }
            }

            // If any invalid types were found
            if (invalidTypes.Any())
            {
                // Format a more specific error message listing the invalid types
                string specificErrorMessage = string.Format(
                    ErrorMessageString, // Use the message provided to the attribute
                    validationContext.DisplayName // {0} placeholder if used in message
                );
                // Append the list of invalid types for better feedback
                specificErrorMessage += $" Invalid types found: {string.Join(", ", invalidTypes)}.";

                // Ensure MemberName is not null
                return new ValidationResult(specificErrorMessage, new[] { validationContext.MemberName! });
            }

            // All non-empty items in the list are valid
            return ValidationResult.Success;
        }
    }

}