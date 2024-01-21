using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class Actor
    {
        public int Id { get; set; }
        public string ProfilePictureURL { get; set; }
        public string FullName { get; set; }
        public string Bio { get; set; }
        public List<Actor_Movie> Actor_Movie { get; set; }
    }
}
