using Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Repository.Service.ShoppingProcessor.Interface.SubInterface
{
    public interface IDiscountExpirationBehavior
    {
        DateTime DoesAllDiscountValid(DateTime now, List<Voucher> couponList);
        DateTime DoesDiscountPromotionValid(DateTime now, IEnumerable<DateTime> voucherExpires);
    }
}
