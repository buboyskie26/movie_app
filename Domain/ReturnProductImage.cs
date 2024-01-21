using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ReturnProductImage
    {
        public int Id { get; set; }

        public ReturnProduct ReturnProduct { get; set; }
        public int ReturnProductId { get; set; }
        public ApplicationUser Consumer { get; set; }
        public string ConsumerId { get; set; }
        public string ProductImage { get; set; }

    }
}
