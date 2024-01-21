using System;
using System.Collections.Generic;
using System.Text;

namespace Domain
{
    public class ProductTransactionResponse
    {
        public int Id { get; set; }
        public ProductTransaction ProductTransaction { get; set; }
        public int ProductTransactionId { get; set; }
        public DateTime ResponseCreation { get; set; }
        public string Message { get; set; }

    }
}
