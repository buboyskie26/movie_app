using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class tyy : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReturnReasons",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnReasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockCount = table.Column<int>(nullable: false),
                    Sold = table.Column<int>(nullable: false),
                    RateReview = table.Column<int>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Price = table.Column<double>(nullable: false),
                    ImageURL = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    VendorId = table.Column<string>(nullable: true),
                    ProductCategory = table.Column<string>(nullable: true),
                    ShippingFee = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovieUserViews",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(nullable: false),
                    ViewDate = table.Column<DateTime>(nullable: false),
                    UserWhoViewId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieUserViews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MovieUserViews_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieUserViews_AspNetUsers_UserWhoViewId",
                        column: x => x.UserWhoViewId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlaceOrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    MovieId = table.Column<int>(nullable: false),
                    UserPlaceOrderId = table.Column<string>(nullable: true),
                    PlacedOrderCreation = table.Column<DateTime>(nullable: false),
                    VendorId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaceOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaceOrderItems_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaceOrderItems_AspNetUsers_UserPlaceOrderId",
                        column: x => x.UserPlaceOrderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaceOrderItems_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RateProducts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(nullable: true),
                    ImageUrl = table.Column<string>(nullable: true),
                    MovieId = table.Column<int>(nullable: false),
                    RateCount = table.Column<int>(nullable: false),
                    IsEnableToComment = table.Column<bool>(nullable: false),
                    RateCreation = table.Column<DateTime>(nullable: false),
                    RateEndedDate = table.Column<DateTime>(nullable: false),
                    UserWhoHadRateId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RateProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RateProducts_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RateProducts_AspNetUsers_UserWhoHadRateId",
                        column: x => x.UserWhoHadRateId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingCartItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MovieId = table.Column<int>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    IsSelected = table.Column<bool>(nullable: false),
                    MyCartUserId = table.Column<string>(nullable: true),
                    VendorId = table.Column<string>(nullable: true),
                    IsCoupon = table.Column<bool>(nullable: false),
                    IsVoucher = table.Column<bool>(nullable: false),
                    DateAddToCart = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingCartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItems_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItems_AspNetUsers_MyCartUserId",
                        column: x => x.MyCartUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingCartItems_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    DiscountPercentage = table.Column<int>(nullable: false),
                    MovieId = table.Column<int>(nullable: false),
                    VendorId = table.Column<string>(nullable: true),
                    Creation = table.Column<DateTime>(nullable: false),
                    Expire = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vouchers_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Vouchers_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<int>(nullable: false),
                    Price = table.Column<double>(nullable: false),
                    MovieId = table.Column<int>(nullable: false),
                    OrdersId = table.Column<int>(nullable: false),
                    DateOrder = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlaceOrderItemsId = table.Column<int>(nullable: false),
                    VendorId = table.Column<string>(nullable: true),
                    ConsumerId = table.Column<string>(nullable: true),
                    RiderId = table.Column<string>(nullable: true),
                    IsOkayForDeliver = table.Column<bool>(nullable: false),
                    OrderPlaced = table.Column<bool>(nullable: false),
                    PaymentConfirmed = table.Column<bool>(nullable: false),
                    IsReturned = table.Column<bool>(nullable: false),
                    OrderReceived = table.Column<bool>(nullable: false),
                    ToRate = table.Column<bool>(nullable: false),
                    Rate = table.Column<int>(nullable: false),
                    DateReceived = table.Column<DateTime>(nullable: false),
                    VendorApproved = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTransactions_AspNetUsers_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductTransactions_PlaceOrderItems_PlaceOrderItemsId",
                        column: x => x.PlaceOrderItemsId,
                        principalTable: "PlaceOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTransactions_AspNetUsers_RiderId",
                        column: x => x.RiderId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductTransactions_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReturnProducts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(nullable: true),
                    PlaceOrderItemsId = table.Column<int>(nullable: false),
                    ReturnReasonsId = table.Column<int>(nullable: false),
                    DateRequest = table.Column<DateTime>(nullable: false),
                    ConsumerId = table.Column<string>(nullable: true),
                    VendorId = table.Column<string>(nullable: true),
                    ReturnedProductApproved = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnProducts_AspNetUsers_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnProducts_PlaceOrderItems_PlaceOrderItemsId",
                        column: x => x.PlaceOrderItemsId,
                        principalTable: "PlaceOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnProducts_ReturnReasons_ReturnReasonsId",
                        column: x => x.ReturnReasonsId,
                        principalTable: "ReturnReasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReturnProducts_AspNetUsers_VendorId",
                        column: x => x.VendorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingVouchers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShoppingCartItemId = table.Column<int>(nullable: true),
                    VoucherId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingVouchers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShoppingVouchers_ShoppingCartItems_ShoppingCartItemId",
                        column: x => x.ShoppingCartItemId,
                        principalTable: "ShoppingCartItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShoppingVouchers_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VoucherNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherId = table.Column<int>(nullable: false),
                    Creation = table.Column<DateTime>(nullable: false),
                    ProductImage = table.Column<string>(nullable: true),
                    Header = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    ReceivingUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherNotifications_AspNetUsers_ReceivingUserId",
                        column: x => x.ReceivingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VoucherNotifications_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductTransactionNotif",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTransactionId = table.Column<int>(nullable: false),
                    Creation = table.Column<DateTime>(nullable: false),
                    ProductImage = table.Column<string>(nullable: true),
                    Header = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    ReceivingUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTransactionNotif", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTransactionNotif_ProductTransactions_ProductTransactionId",
                        column: x => x.ProductTransactionId,
                        principalTable: "ProductTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductTransactionNotif_AspNetUsers_ReceivingUserId",
                        column: x => x.ReceivingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductTransactionResponses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductTransactionId = table.Column<int>(nullable: false),
                    ResponseCreation = table.Column<DateTime>(nullable: false),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTransactionResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductTransactionResponses_ProductTransactions_ProductTransactionId",
                        column: x => x.ProductTransactionId,
                        principalTable: "ProductTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnProductId = table.Column<int>(nullable: false),
                    ConsumerId = table.Column<string>(nullable: true),
                    ProductImage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnProductImages_AspNetUsers_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnProductImages_ReturnProducts_ReturnProductId",
                        column: x => x.ReturnProductId,
                        principalTable: "ReturnProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReturnProductNotifications",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReturnProductId = table.Column<int>(nullable: false),
                    Creation = table.Column<DateTime>(nullable: false),
                    ProductImage = table.Column<string>(nullable: true),
                    Header = table.Column<string>(nullable: true),
                    MessageBody = table.Column<string>(nullable: true),
                    ReceivingUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReturnProductNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReturnProductNotifications_AspNetUsers_ReceivingUserId",
                        column: x => x.ReceivingUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReturnProductNotifications_ReturnProducts_ReturnProductId",
                        column: x => x.ReturnProductId,
                        principalTable: "ReturnProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_VendorId",
                table: "Movies",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieUserViews_MovieId",
                table: "MovieUserViews",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_MovieUserViews_UserWhoViewId",
                table: "MovieUserViews",
                column: "UserWhoViewId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_MovieId",
                table: "OrderItems",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrdersId",
                table: "OrderItems",
                column: "OrdersId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceOrderItems_MovieId",
                table: "PlaceOrderItems",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceOrderItems_UserPlaceOrderId",
                table: "PlaceOrderItems",
                column: "UserPlaceOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaceOrderItems_VendorId",
                table: "PlaceOrderItems",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactionNotif_ProductTransactionId",
                table: "ProductTransactionNotif",
                column: "ProductTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactionNotif_ReceivingUserId",
                table: "ProductTransactionNotif",
                column: "ReceivingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactionResponses_ProductTransactionId",
                table: "ProductTransactionResponses",
                column: "ProductTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactions_ConsumerId",
                table: "ProductTransactions",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactions_PlaceOrderItemsId",
                table: "ProductTransactions",
                column: "PlaceOrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactions_RiderId",
                table: "ProductTransactions",
                column: "RiderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductTransactions_VendorId",
                table: "ProductTransactions",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_RateProducts_MovieId",
                table: "RateProducts",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_RateProducts_UserWhoHadRateId",
                table: "RateProducts",
                column: "UserWhoHadRateId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProductImages_ConsumerId",
                table: "ReturnProductImages",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProductImages_ReturnProductId",
                table: "ReturnProductImages",
                column: "ReturnProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProductNotifications_ReceivingUserId",
                table: "ReturnProductNotifications",
                column: "ReceivingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProductNotifications_ReturnProductId",
                table: "ReturnProductNotifications",
                column: "ReturnProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProducts_ConsumerId",
                table: "ReturnProducts",
                column: "ConsumerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProducts_PlaceOrderItemsId",
                table: "ReturnProducts",
                column: "PlaceOrderItemsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProducts_ReturnReasonsId",
                table: "ReturnProducts",
                column: "ReturnReasonsId");

            migrationBuilder.CreateIndex(
                name: "IX_ReturnProducts_VendorId",
                table: "ReturnProducts",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_MovieId",
                table: "ShoppingCartItems",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_MyCartUserId",
                table: "ShoppingCartItems",
                column: "MyCartUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingCartItems_VendorId",
                table: "ShoppingCartItems",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingVouchers_ShoppingCartItemId",
                table: "ShoppingVouchers",
                column: "ShoppingCartItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingVouchers_VoucherId",
                table: "ShoppingVouchers",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherNotifications_ReceivingUserId",
                table: "VoucherNotifications",
                column: "ReceivingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherNotifications_VoucherId",
                table: "VoucherNotifications",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_MovieId",
                table: "Vouchers",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VendorId",
                table: "Vouchers",
                column: "VendorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "MovieUserViews");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "ProductTransactionNotif");

            migrationBuilder.DropTable(
                name: "ProductTransactionResponses");

            migrationBuilder.DropTable(
                name: "RateProducts");

            migrationBuilder.DropTable(
                name: "ReturnProductImages");

            migrationBuilder.DropTable(
                name: "ReturnProductNotifications");

            migrationBuilder.DropTable(
                name: "ShoppingVouchers");

            migrationBuilder.DropTable(
                name: "VoucherNotifications");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductTransactions");

            migrationBuilder.DropTable(
                name: "ReturnProducts");

            migrationBuilder.DropTable(
                name: "ShoppingCartItems");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "PlaceOrderItems");

            migrationBuilder.DropTable(
                name: "ReturnReasons");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
