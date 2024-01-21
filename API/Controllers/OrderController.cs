using Application.ViewModel.Movie;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            this.mapper = mapper;
            _userManager = userManager;
        }
        [HttpGet("PlaceCheckoutItems")]
        public async Task<ActionResult<ProductsOrderedList>> PlaceCheckoutItems()
        {
            var user = await _userManager.GetUserAsync(User);

            var productOrdered = await (from s in _context.Movies
                                        join cart in _context.ShoppingCartItems on s.Id equals cart.MovieId
                                        where cart.MyCartUserId == user.Id
                                        where cart.IsSelected == true

                                        select new MovieCheckoutList()
                                        {

                                             Amount = cart.Amount,
                                             MovieId=cart.MovieId,
                                             MovieName=cart.Movie.Name,
                                             UserName = cart.MyCartUser.FirstName+" "+ cart.MyCartUser.LastName,
                                             UnitPrice = Math.Round(cart.Price, 2),
                                             DiscountedPrice = s.Price > cart.Price ? Math.Round(s.Price - cart.Price, 2) : 0.00
                                        }).ToListAsync();

            return new ProductsOrderedList()
            {
                ProductsOrdered = productOrdered,
                TotalPrice = Math.Round(productOrdered.Select(w=> w.Amount * w.UnitPrice).Sum(), 2),
            };
        }
 
    }
}
