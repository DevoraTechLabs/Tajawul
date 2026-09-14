using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Helpers.Filters.SocialMedia
{
    public class PostsFilter
    {
        private int _pageSize = 10;
        private int _pageNumber = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1; // Ensure it's at least 1
        }

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 0 && value <= 100) ? value : 10; //Ensure is between 1-100
        }
    }
}
