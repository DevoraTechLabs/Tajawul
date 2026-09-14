using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.user.Translation
{
    public class TranslationItemDto
    {
        [Required(ErrorMessage = "Source text is required")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Source text must be between 3 and 1000 characters")]
        public required string SourceText { get; set; }

        [Required(ErrorMessage = "Input language is required")]
        [TranslationLanguageCodeExits("Input language is not supported. Please select a supported language code.")]
        public required string InputLanguage { get; set; }

        [Required(ErrorMessage = "Output language is required")]
        [TranslationLanguageCodeExits("Output language is not supported. Please select a supported language code.")]
        public required string OutputLanguage { get; set; }

    }
}