using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Tajawul.Models.Domain.General;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class UniqueSocialMediaLinksAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle with [Required] if needed
            }

            if (value is not List<SocialMediaLink> socialMediaLinks)
            {
                return new ValidationResult("Value must be a list of SocialMediaLink objects.");
            }

            var uniqueLinks = new HashSet<string>(socialMediaLinks.Select(l => l.Url.ToLowerInvariant()));

            if (uniqueLinks.Count < socialMediaLinks.Count)
            {
                return new ValidationResult(ErrorMessage ?? "Social Media Links must be unique.");
            }

            return ValidationResult.Success;
        }
    }
}