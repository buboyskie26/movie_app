using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Movie
{
    public class SearchingProductView
    {
        public string SearchQuery { get; set; }
        public IEnumerable<ProductInformationDTO> ProductInformation { get; set; }

    }
  
}
