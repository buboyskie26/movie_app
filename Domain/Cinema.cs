using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Cinema
    {
        public int Id { get; set; }
        public string Logo { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Movie> Movie { get; set; }
    }
}
