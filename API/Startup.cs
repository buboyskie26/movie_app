using Application.BaseRepository;
using Application.Helper;
using Application.Mapper;
using Application.Repository.Factory.PlaceOrder;
using Application.Repository.IService;
using Application.Repository.Service;
using Application.Repository.Service.ShoppingProcessor;
using Application.Repository.Service.ShoppingProcessor.Interface;
using Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EcommerceAPI", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Insert Token here",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            services.AddHttpContextAccessor();
          /*  services.AddControllersWithViews();*/
            /*services.AddMemoryCache();*/
            /*services.AddSession();*/
/*            services.AddAuthentication(o =>
            {
                o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });*/
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
             .AddJwtBearer(options =>
             {
                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = false,
                     ValidateAudience = false,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(Configuration["keyjwt"])),
                     ClockSkew = TimeSpan.Zero
                 };
             });
           

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(Configuration.GetConnectionString("DevConnection")));

            services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddScoped<IFileStorageService, InAppStorageService>();
            services.AddScoped<IShoppingCart, ShoppingCartRepository>();
            services.AddScoped<IReturnProduct, ReturnProductRepository>();
            services.AddScoped<IDiscountedShop, DiscountedShopRepository>();
            services.AddScoped<IShoppingDiscountHall, ShoppingDiscountHall>();
            services.AddScoped<IPlaceOrder, PlacerOrderRepository>();
            services.AddScoped<IVoucher, VoucherRepository>();
            services.AddScoped<IAccount, AccountRepository>();
            services.AddScoped<IMessageNotification, MessageNotificationRepository>();
            services.AddScoped<IMessageTable, MessageTableRepository>();
            /*services.AddScoped<ISampFactory, SampFactory>();*/

            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            /*services.AddScoped(IBaseRepository<>, BaseRepository<>);*/

            /*services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();*/
            /*services.AddScoped(i => ShoppingCart.GetShoppingCart(i));*/
            services.AddCors(options =>
            {
                var frontEndUrl = Configuration.GetValue<string>("frontend-url");
                options.AddDefaultPolicy(b =>
                {
                    b.WithOrigins(frontEndUrl).AllowAnyMethod().AllowAnyHeader()
                        .WithExposedHeaders(new string[] { "totalAmountOfRecords" });
                });

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcommerceAPI v1"));

            }
            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
