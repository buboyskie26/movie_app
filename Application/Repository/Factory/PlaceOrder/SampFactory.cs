using Application.Repository.IService;
using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Factory.PlaceOrder
{
    public interface ISampFactory
    {
        Task CartBehavior(ApplicationUser user, List<PlaceOrderItems> withAndNonQuotaPlaceOrderList,
            string VENDOR_SHIP_THE_PARCEL, DateTime now);
    }
     
    public class SampFactory : ISampFactory
    {
        private readonly IPlaceOrder _placerOrder;
        private readonly ApplicationDbContext _context;

        public SampFactory(IPlaceOrder placerOrder, ApplicationDbContext context)
        {
            _placerOrder = placerOrder;
        }

        public async Task CartBehavior(ApplicationUser user,
            List<PlaceOrderItems> withAndNonQuotaPlaceOrderList,
            string VENDOR_SHIP_THE_PARCEL, DateTime now)
        {
            await _context.PlaceOrderItems.AddRangeAsync(withAndNonQuotaPlaceOrderList);
            await _context.SaveChangesAsync();

           
            await _placerOrder.AllInProductUserVendorResponse(user, withAndNonQuotaPlaceOrderList,
                VENDOR_SHIP_THE_PARCEL, now);
            await _context.SaveChangesAsync();
        }
    }
}
