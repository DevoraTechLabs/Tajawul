namespace Tajawul.Helpers.ValidationAttributes
{
    public class MaxImageFileSizeAttribute : MaxFileSizeAttribute
    {

        public MaxImageFileSizeAttribute(int maxImageSizeInMB, string? errorMessage = null)
            : base(maxImageSizeInMB)
        {
            ErrorMessage = errorMessage ?? $"The image file size exceeds the maximum allowed size of {maxImageSizeInMB} MB.";
        }
    }
}