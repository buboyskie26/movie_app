using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.ShoppingCartRepoRefactor
{
    public interface Specification<T>
    {
        bool IsSatisfied(T t);
    }

    public class VoucherSpecification : Specification<Voucher>
    {
        private Voucher voucher;
        private ShoppingCartItem shoppingCartItem;
        public VoucherSpecification(Voucher voucher, ShoppingCartItem shoppingCartItem)
        {
            this.voucher = voucher;
            this.shoppingCartItem = shoppingCartItem;
        }

        public bool IsSatisfied(Voucher t)
        {
            return shoppingCartItem.MovieId == t.MovieId;
        }
    }
}
