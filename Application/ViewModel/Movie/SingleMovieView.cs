using Application.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class SingleMovieView
    {
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
        public int MovieId { get; set; }
        public int RateCount { get; set; }
        public bool WithMedia { get; set; }

    }
}
