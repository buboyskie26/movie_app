using Application.Repository.IService;
using Domain;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository.Factory.PlaceOrder
{
    public interface IPlaceOrderFactory
    {
        Task CartHandleWithOrWithoutQuota(List<PlaceOrderItems> withAndNonQuotaPlaceOrderList,
            bool myFinalCartWithQuota, bool myFinalCartWithoutQuota);
        Task<List<PlaceOrderItems>> Samtm(ApplicationUser user, string VENDOR_SHIP_THE_PARCEL,
            DateTime now, int myFinalCartWithoutQuotaCount, int myFinalCartWithQuotaCount,
            List<PlaceOrderItems> placementWithQuotaOrderList,
            List<PlaceOrderItems> placementWithoutQuotaOrderList,
            List<PlaceOrderItems> withAndNonQuotaPlaceOrderList);


    }
    public class PlaceOrderFactory : IPlaceOrderFactory
    {
        private readonly IPlaceOrder _placeOrder;
        private readonly ApplicationDbContext _context;

        public PlaceOrderFactory(IPlaceOrder placeOrder, ApplicationDbContext context)
        {
            _placeOrder = placeOrder;
            _context = context;
        }

        public async Task CartHandleWithOrWithoutQuota(List<PlaceOrderItems> withAndNonQuotaPlaceOrderList,
            bool myFinalCartWithQuota, bool myFinalCartWithoutQuota)
        {
            if(myFinalCartWithQuota == true && myFinalCartWithoutQuota == true)
            {
               
            }
            else if (myFinalCartWithQuota == true && myFinalCartWithoutQuota == false)
            {

            }
            else if (myFinalCartWithoutQuota == true && myFinalCartWithQuota == false)
            {

            }
            /*if (myFinalCartWithQuota.Count != 0 && myFinalCartWithoutQuota.Count != 0)
            {
                withAndNonQuotaPlaceOrderList = placementWithQuotaOrderList
                    .Concat(placementWithoutQuotaOrderList).ToList();

                await _context.PlaceOrderItems.AddRangeAsync(withAndNonQuotaPlaceOrderList);
                await _context.SaveChangesAsync();

                *//* test
                 * var  productTransaction = await _placeOrder.AddingProductTransaction(user.Id, withAndNonQuotaPlaceOrderList);

                // The notification depends on how many would be the quantity of
                // PlaceOrderItems of the customer
                // Notification for customer
                await _placeOrder.AddingProductCustomerNotification(user.Id, now, productTransaction);
                await _placeOrder.AddingProductVendorNotification(user, now, productTransaction);
                await _placeOrder.AddingProductTransactionReponse(VENDOR_SHIP_THE_PARCEL, now, productTransaction);*//*
                await _placeOrder.AllInProductUserVendorResponse(user, withAndNonQuotaPlaceOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);

                await _context.SaveChangesAsync();

            }
            else if (myFinalCartWithQuota.Count != 0 && myFinalCartWithoutQuota.Count == 0)
            {
                await _context.PlaceOrderItems.AddRangeAsync(placementWithQuotaOrderList);
                await _context.SaveChangesAsync();

                await _placeOrder.AllInProductUserVendorResponse(user, placementWithQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);

                await _context.SaveChangesAsync();
            }
            else if (myFinalCartWithoutQuota.Count != 0 && myFinalCartWithQuota.Count == 0)
            {
                await _context.PlaceOrderItems.AddRangeAsync(placementWithoutQuotaOrderList);
                await _context.SaveChangesAsync();

                await _placeOrder.AllInProductUserVendorResponse(user, placementWithoutQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);

                await _context.SaveChangesAsync();
            }*/
        }

        public async Task<List<PlaceOrderItems>> Samtm(ApplicationUser user, string VENDOR_SHIP_THE_PARCEL,
            DateTime now, int myFinalCartWithoutQuotaCount, int myFinalCartWithQuotaCount,
            List<PlaceOrderItems> placementWithQuotaOrderList,
            List<PlaceOrderItems> placementWithoutQuotaOrderList,
            List<PlaceOrderItems> withAndNonQuotaPlaceOrderList)
        {
            if (myFinalCartWithQuotaCount != 0 && myFinalCartWithoutQuotaCount != 0)
            {
                withAndNonQuotaPlaceOrderList = placementWithQuotaOrderList
                    .Concat(placementWithoutQuotaOrderList).ToList();

                await _context.PlaceOrderItems.AddRangeAsync(withAndNonQuotaPlaceOrderList);
                await _context.SaveChangesAsync();

                /* test
                 * var  productTransaction = await _placeOrder.AddingProductTransaction(user.Id, withAndNonQuotaPlaceOrderList);

                // The notification depends on how many would be the quantity of
                // PlaceOrderItems of the customer
                // Notification for customer
                await _placeOrder.AddingProductCustomerNotification(user.Id, now, productTransaction);
                await _placeOrder.AddingProductVendorNotification(user, now, productTransaction);
                await _placeOrder.AddingProductTransactionReponse(VENDOR_SHIP_THE_PARCEL, now, productTransaction);*/
                await _placeOrder.AllInProductUserVendorResponse(user, withAndNonQuotaPlaceOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);

                await _context.SaveChangesAsync();

            }
            else if (myFinalCartWithQuotaCount != 0 && myFinalCartWithoutQuotaCount == 0)
            {
                await _context.PlaceOrderItems.AddRangeAsync(placementWithQuotaOrderList);
                await _context.SaveChangesAsync();

                await _placeOrder.AllInProductUserVendorResponse(user, placementWithQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);

                await _context.SaveChangesAsync();
            }
            else if (myFinalCartWithoutQuotaCount != 0 && myFinalCartWithQuotaCount == 0)
            {
                await _context.PlaceOrderItems.AddRangeAsync(placementWithoutQuotaOrderList);
                await _context.SaveChangesAsync();

                await _placeOrder.AllInProductUserVendorResponse(user, placementWithoutQuotaOrderList,
                    VENDOR_SHIP_THE_PARCEL, now);

                await _context.SaveChangesAsync();
            }

            return withAndNonQuotaPlaceOrderList;
        }
    }
}
