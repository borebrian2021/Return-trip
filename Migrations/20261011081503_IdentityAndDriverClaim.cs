using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Return_trip.Migrations
{
    /// <inheritdoc />
    public partial class IdentityAndDriverClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdminCommissionSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Percentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdminCommissionSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false)
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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                name: "GoodsOwners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoodsOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GoodsOwners_AspNetUsers_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleOwners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BusinessRegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleOwners_AspNetUsers_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Cargoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GoodsOwnerId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FromPlaceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FromLatitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    FromLongitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    ToLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ToPlaceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ToLatitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    ToLongitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    RequiredDepartureAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryDeadlineAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DepartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetentionExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReopenedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    WeightKg = table.Column<decimal>(type: "decimal(12,3)", precision: 12, scale: 3, nullable: false),
                    PackageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricingMode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PricePerKg = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    TransportCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CommissionPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    RetentionPeriodDays = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdvancePaymentPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    IsBidOpen = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargoes", x => x.Id);
                    table.CheckConstraint("CK_Cargo_ActualDates", "[DepartedAt] IS NULL OR [DeliveredAt] IS NULL OR [DeliveredAt] >= [DepartedAt]");
                    table.CheckConstraint("CK_Cargo_FromCoordinates", "([FromLatitude] IS NULL OR [FromLatitude] BETWEEN -90 AND 90) AND ([FromLongitude] IS NULL OR [FromLongitude] BETWEEN -180 AND 180)");
                    table.CheckConstraint("CK_Cargo_PricingMode", "([PricingMode] = 'FixedTotal' AND [PricePerKg] IS NULL) OR ([PricingMode] = 'PerKg' AND [WeightKg] > 0 AND [PricePerKg] > 0)");
                    table.CheckConstraint("CK_Cargo_RetentionExpiry", "[RetentionExpiresAt] IS NULL OR [RequiredDepartureAt] IS NULL OR [RetentionExpiresAt] >= [RequiredDepartureAt]");
                    table.CheckConstraint("CK_Cargo_ScheduledDates", "[RequiredDepartureAt] IS NULL OR [DeliveryDeadlineAt] IS NULL OR [DeliveryDeadlineAt] >= [RequiredDepartureAt]");
                    table.CheckConstraint("CK_Cargo_ToCoordinates", "([ToLatitude] IS NULL OR [ToLatitude] BETWEEN -90 AND 90) AND ([ToLongitude] IS NULL OR [ToLongitude] BETWEEN -180 AND 180)");
                    table.ForeignKey(
                        name: "FK_Cargoes_GoodsOwners_GoodsOwnerId",
                        column: x => x.GoodsOwnerId,
                        principalTable: "GoodsOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BankDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleOwnerId = table.Column<int>(type: "int", nullable: true),
                    GoodsOwnerId = table.Column<int>(type: "int", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SwiftCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankDetails_GoodsOwners_GoodsOwnerId",
                        column: x => x.GoodsOwnerId,
                        principalTable: "GoodsOwners",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BankDetails_VehicleOwners_VehicleOwnerId",
                        column: x => x.VehicleOwnerId,
                        principalTable: "VehicleOwners",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyRegistrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleOwnerId = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberPlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificateOfRegistrationUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CertificateOfRegistrationType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyRegistrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyRegistrations_VehicleOwners_VehicleOwnerId",
                        column: x => x.VehicleOwnerId,
                        principalTable: "VehicleOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Drivers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: true),
                    VehicleOwnerId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseImageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drivers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drivers_AspNetUsers_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Drivers_VehicleOwners_VehicleOwnerId",
                        column: x => x.VehicleOwnerId,
                        principalTable: "VehicleOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: true),
                    VehicleOwnerId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RelatedEntityType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedEntityId = table.Column<int>(type: "int", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.CheckConstraint("CK_Notifications_ExactlyOneRecipient", "(([UserAccountId] IS NOT NULL AND [VehicleOwnerId] IS NULL) OR ([UserAccountId] IS NULL AND [VehicleOwnerId] IS NOT NULL))");
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notifications_VehicleOwners_VehicleOwnerId",
                        column: x => x.VehicleOwnerId,
                        principalTable: "VehicleOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAgents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAccountId = table.Column<int>(type: "int", nullable: false),
                    VehicleOwnerId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAgents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAgents_AspNetUsers_UserAccountId",
                        column: x => x.UserAccountId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyAgents_CompanyRegistrations_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "CompanyRegistrations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyAgents_VehicleOwners_VehicleOwnerId",
                        column: x => x.VehicleOwnerId,
                        principalTable: "VehicleOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleOwnerId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseDocumentUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseDocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleDetails_CompanyRegistrations_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "CompanyRegistrations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleDetails_VehicleOwners_VehicleOwnerId",
                        column: x => x.VehicleOwnerId,
                        principalTable: "VehicleOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleRoutes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleDetailId = table.Column<int>(type: "int", nullable: false),
                    CompanyAgentId = table.Column<int>(type: "int", nullable: false),
                    FromLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FromPlaceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FromLatitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    FromLongitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    ToLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ToPlaceId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ToLatitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    ToLongitude = table.Column<decimal>(type: "decimal(9,6)", precision: 9, scale: 6, nullable: true),
                    TripStartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedArrivalAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnAvailabilityStartsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReturnAvailabilityEndsAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsAwaitingReturn = table.Column<bool>(type: "bit", nullable: false),
                    RoutePathJson = table.Column<string>(type: "nvarchar(max)", maxLength: 1000000, nullable: true),
                    MapProvider = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRoutes", x => x.Id);
                    table.CheckConstraint("CK_VehicleRoutes_FromCoordinates", "([FromLatitude] IS NULL OR [FromLatitude] BETWEEN -90 AND 90) AND ([FromLongitude] IS NULL OR [FromLongitude] BETWEEN -180 AND 180)");
                    table.CheckConstraint("CK_VehicleRoutes_ReturnWindow", "[ReturnAvailabilityEndsAt] IS NULL OR [ReturnAvailabilityStartsAt] IS NULL OR [ReturnAvailabilityEndsAt] >= [ReturnAvailabilityStartsAt]");
                    table.CheckConstraint("CK_VehicleRoutes_ToCoordinates", "([ToLatitude] IS NULL OR [ToLatitude] BETWEEN -90 AND 90) AND ([ToLongitude] IS NULL OR [ToLongitude] BETWEEN -180 AND 180)");
                    table.CheckConstraint("CK_VehicleRoutes_TripTimes", "[ExpectedArrivalAt] >= [TripStartsAt]");
                    table.ForeignKey(
                        name: "FK_VehicleRoutes_CompanyAgents_CompanyAgentId",
                        column: x => x.CompanyAgentId,
                        principalTable: "CompanyAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleRoutes_VehicleDetails_VehicleDetailId",
                        column: x => x.VehicleDetailId,
                        principalTable: "VehicleDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CargoBids",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoId = table.Column<int>(type: "int", nullable: false),
                    CompanyAgentId = table.Column<int>(type: "int", nullable: false),
                    VehicleRouteId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: true),
                    BidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    SuggestedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    SuggestionComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PriceSuggestionStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PriceDecisionAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PriceDecidedByGoodsOwnerId = table.Column<int>(type: "int", nullable: true),
                    EstimatedVehicleArrivalAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoBids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoBids_Cargoes_CargoId",
                        column: x => x.CargoId,
                        principalTable: "Cargoes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CargoBids_CompanyAgents_CompanyAgentId",
                        column: x => x.CompanyAgentId,
                        principalTable: "CompanyAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CargoBids_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CargoBids_GoodsOwners_PriceDecidedByGoodsOwnerId",
                        column: x => x.PriceDecidedByGoodsOwnerId,
                        principalTable: "GoodsOwners",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_CargoBids_VehicleRoutes_VehicleRouteId",
                        column: x => x.VehicleRouteId,
                        principalTable: "VehicleRoutes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryReceipts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoBidId = table.Column<int>(type: "int", nullable: false),
                    DriverId = table.Column<int>(type: "int", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SignedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmationStatus = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConfirmedByCompanyAgentId = table.Column<int>(type: "int", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AgentComments = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryReceipts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryReceipts_CargoBids_CargoBidId",
                        column: x => x.CargoBidId,
                        principalTable: "CargoBids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DeliveryReceipts_CompanyAgents_ConfirmedByCompanyAgentId",
                        column: x => x.ConfirmedByCompanyAgentId,
                        principalTable: "CompanyAgents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DeliveryReceipts_Drivers_DriverId",
                        column: x => x.DriverId,
                        principalTable: "Drivers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_GoodsOwnerId",
                table: "BankDetails",
                column: "GoodsOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_BankDetails_VehicleOwnerId",
                table: "BankDetails",
                column: "VehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_CargoId_CompanyAgentId",
                table: "CargoBids",
                columns: new[] { "CargoId", "CompanyAgentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_CompanyAgentId",
                table: "CargoBids",
                column: "CompanyAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_DriverId",
                table: "CargoBids",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_PriceDecidedByGoodsOwnerId",
                table: "CargoBids",
                column: "PriceDecidedByGoodsOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_Status_CreatedAt",
                table: "CargoBids",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CargoBids_VehicleRouteId",
                table: "CargoBids",
                column: "VehicleRouteId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargoes_GoodsOwnerId",
                table: "Cargoes",
                column: "GoodsOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Cargoes_Status_RetentionExpiresAt",
                table: "Cargoes",
                columns: new[] { "Status", "RetentionExpiresAt" });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAgents_CompanyId",
                table: "CompanyAgents",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAgents_UserAccountId",
                table: "CompanyAgents",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAgents_VehicleOwnerId",
                table: "CompanyAgents",
                column: "VehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyRegistrations_VehicleOwnerId",
                table: "CompanyRegistrations",
                column: "VehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryReceipts_CargoBidId",
                table: "DeliveryReceipts",
                column: "CargoBidId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryReceipts_ConfirmationStatus",
                table: "DeliveryReceipts",
                column: "ConfirmationStatus");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryReceipts_ConfirmedByCompanyAgentId",
                table: "DeliveryReceipts",
                column: "ConfirmedByCompanyAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryReceipts_DriverId",
                table: "DeliveryReceipts",
                column: "DriverId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_IdNumber",
                table: "Drivers",
                column: "IdNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_UserAccountId",
                table: "Drivers",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Drivers_VehicleOwnerId",
                table: "Drivers",
                column: "VehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_GoodsOwners_UserAccountId",
                table: "GoodsOwners",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserAccountId_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "UserAccountId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_VehicleOwnerId",
                table: "Notifications",
                column: "VehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDetails_CompanyId",
                table: "VehicleDetails",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleDetails_VehicleOwnerId",
                table: "VehicleDetails",
                column: "VehicleOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleOwners_UserAccountId",
                table: "VehicleOwners",
                column: "UserAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRoutes_CompanyAgentId",
                table: "VehicleRoutes",
                column: "CompanyAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRoutes_IsAwaitingReturn_ReturnAvailabilityStartsAt_ReturnAvailabilityEndsAt",
                table: "VehicleRoutes",
                columns: new[] { "IsAwaitingReturn", "ReturnAvailabilityStartsAt", "ReturnAvailabilityEndsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRoutes_VehicleDetailId",
                table: "VehicleRoutes",
                column: "VehicleDetailId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdminCommissionSettings");

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
                name: "BankDetails");

            migrationBuilder.DropTable(
                name: "DeliveryReceipts");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CargoBids");

            migrationBuilder.DropTable(
                name: "Cargoes");

            migrationBuilder.DropTable(
                name: "Drivers");

            migrationBuilder.DropTable(
                name: "VehicleRoutes");

            migrationBuilder.DropTable(
                name: "GoodsOwners");

            migrationBuilder.DropTable(
                name: "CompanyAgents");

            migrationBuilder.DropTable(
                name: "VehicleDetails");

            migrationBuilder.DropTable(
                name: "CompanyRegistrations");

            migrationBuilder.DropTable(
                name: "VehicleOwners");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
