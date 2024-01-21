using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ViewModel.Voucher
{
    public class VoucherCreationDTO
    {
        public int Quantity { get; set; }
        public int DiscountPercentage { get; set; }
        public int MovieId { get; set; }
        // nullable
        public string VoucherCode { get; set; }
    }
    public class DiscountedShopDTO
    {
        public int Quantity { get; set; }
        public int MinimumSpendQuota { get; set; }
        public int FixedPrice { get; set; }
    }
}
