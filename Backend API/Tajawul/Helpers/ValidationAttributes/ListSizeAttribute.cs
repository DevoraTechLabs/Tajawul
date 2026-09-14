using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ListSizeAttribute : ValidationAttribute
    {
        public int Minimum { get; }
        public int Maximum { get; }

        public ListSizeAttribute(int minimum , int maximum )
        {
            if (minimum < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(minimum), "Minimum length must be a non-negative integer.");
            }
            if (maximum < minimum)
            {
                throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum length must be greater than or equal to minimum length.");
            }

            Minimum = minimum;
            Maximum = maximum;

            // Set default error messages if not provided by the user
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                if (Minimum > 0 && Maximum < int.MaxValue)
                {
                    ErrorMessage = $"The field {{0}} must have between {Minimum} and {Maximum} items.";
                }
                else if (Minimum > 0)
                {
                    ErrorMessage = $"The field {{0}} must have at least {Minimum} item(s).";
                }
                else if (Maximum < int.MaxValue)
                {
                    ErrorMessage = $"The field {{0}} must have at most {Maximum} item(s).";
                }
            }
        }

        public override bool IsValid(object? value)
        {
            // Null values are handled by [Required] if needed separately
            if (value == null)
            {
                // If minimum is 0, null is okay. If minimum > 0, it's not.
                // However, usually you'd use [Required] for null checks, so we often return true here.
                // Let's strictly enforce the minimum count even for null lists if Min > 0
                return Minimum == 0;
            }

            if (value is IList list)
            {
                return list.Count >= Minimum && list.Count <= Maximum;
            }

            // Not a list type this attribute can validate
            return false;
            // OR throw an exception if you want to be stricter about usage:
            // throw new InvalidOperationException($"ListSizeAttribute can only be used on types implementing IList.");
        }
    }
}