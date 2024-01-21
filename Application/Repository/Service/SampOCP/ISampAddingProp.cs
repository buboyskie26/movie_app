using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repository.Service.SampOCP
{
    
    public interface ISampAddingProp
    {
        string UserId { get; set; }
        ShoppingCartItem Cart { get; set; }
        Movie Movie { get; set; }
        decimal FinalPrice{ get; set; }
        int MovieShipping{ get; set; }
        decimal TotalDiscount{ get; set; }
        DateTime Now { get; set; }
        IAddingBehavior ISampAdding { get; set; }

    }

}
