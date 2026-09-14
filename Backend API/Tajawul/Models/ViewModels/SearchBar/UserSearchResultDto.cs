using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tajawul.Models.ViewModels.SearchBar
{

    public class UserSearchResultDto
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsTopTraveler { get; set; }
    }
}