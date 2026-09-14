using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.Filters
{
    public class RecommendationFilter
    {
        private int _pageNumber = 1;
        private int _pageSize = 10;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 0 && value <= 100) ? value : 10;
        }

    }
}