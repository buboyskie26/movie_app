using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain
{
    public class DiscountShop_Cart
    {
        public int Id { get; set; }

        [ForeignKey("ShoppingCartItemId")]
        public ShoppingCartItem ShoppingCartItem { get; set; }
        public int ShoppingCartItemId { get; set; }
        [ForeignKey("DiscountedShopId")]
        public DiscountedShop DiscountedShop { get; set; }
        public int DiscountedShopId { get; set; }

        public ApplicationUser ShoppingVoucherUser { get; set; }
        public string ShoppingVoucherUserid { get; set; }
    }
}
