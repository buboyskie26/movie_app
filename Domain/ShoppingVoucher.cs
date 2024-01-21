using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class ShoppingVoucher
    {
        public int Id { get; set; }
        [ForeignKey("ShoppingCartItemId")]
        public ShoppingCartItem ShoppingCartItem { get; set; }
        public int? ShoppingCartItemId { get; set; }
        [ForeignKey("VoucherId")]
        public Voucher Voucher { get; set; }
        public int? VoucherId { get; set; }

        public ApplicationUser ShoppingVoucherUser { get; set; }
        public string ShoppingVoucherUserid { get; set; }


    }
}
