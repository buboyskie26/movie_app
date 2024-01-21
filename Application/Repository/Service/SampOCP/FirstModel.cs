using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repository.Service.SampOCP
{
    public class FirstModel : ISampAddingProp
    {
        public string UserId { get; set; }
        public ShoppingCartItem Cart { get; set; }
        public Movie Movie { get; set; }
        public decimal FinalPrice { get; set; }
        public int MovieShipping { get; set; }
        public decimal TotalDiscount { get; set; }
        public DateTime Now { get; set; }
        public IAddingBehavior ISampAdding { get; set; } = new FirstAdding();
    }
}
