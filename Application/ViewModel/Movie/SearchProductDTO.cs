using Application.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class SearchProductDTO
    {
        public string keyword { get; set; }
        public bool highestToLowest { get; set; }
        public bool lowestToHighest { get; set; }
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationDTO PaginationDTO
        {
            get
            {
                return new PaginationDTO()
                {
                    Page = Page,
                    RecordsPerPage = RecordsPerPage
                };
            }
        }
    }
}
