using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.ReturnRequest
{
    public class ReturnRequestCreationDTO
    {
        /*public IFormFile ProductImage { get; set; }*/
        public string Description { get; set; }
        public int PlaceOrderItemsId { get; set; }
        public int ReturnReasonsId { get; set; }
        public List<IFormFile> ProductImages { get; set; }

    }
    public class ReturnReasonCreationDTO
    {
        public string Reason { get; set; }
 
    }
}
