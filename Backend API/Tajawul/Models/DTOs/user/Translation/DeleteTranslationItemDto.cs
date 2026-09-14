using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.user.Translation
{
    public class DeleteTranslationItemDto
    {

        [Required(ErrorMessage = "Translation item id is required")]
        [MongoObjectId]

        public required string TranslationItemId { get; set; }
    }
}
