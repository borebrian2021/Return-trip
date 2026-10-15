using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class SystemDBContext : IdentityDbContext<UserAccount, IdentityRole<int>, int>
{
    public SystemDBContext(DbContextOptions<SystemDBContext> options) : base(options)
    {
    }

    public DbSet<UserAccount> UserAccounts { get; set; }
    public DbSet<VehicleOwner> VehicleOwners { get; set; }
    public DbSet<CompanyRegistration> CompanyRegistrations { get; set; }
    public DbSet<CompanyAgent> CompanyAgents { get; set; }
    public DbSet<VehicleDetail> VehicleDetails { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<GoodsOwner> GoodsOwners { get; set; }
    public DbSet<Cargo> Cargoes { get; set; }
    public DbSet<CargoBid> CargoBids { get; set; }
    public DbSet<VehicleRoute> VehicleRoutes { get; set; }
    public DbSet<DeliveryReceipt> DeliveryReceipts { get; set; }
    public DbSet<BankDetail> BankDetails { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<AdminCommissionSetting> AdminCommissionSettings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserAccount>().ToTable("UserAccounts");
        modelBuilder.Entity<UserAccount>()
            .HasIndex(u => u.NormalizedEmail)
            .IsUnique()
            .HasFilter("[NormalizedEmail] IS NOT NULL");

        modelBuilder.Entity<VehicleOwner>()
            .HasOne(v => v.UserAccount)
            .WithMany(u => u.VehicleOwners)
            .HasForeignKey(v => v.UserAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CompanyRegistration>()
            .HasOne(c => c.VehicleOwner)
            .WithMany(v => v.Companies)
            .HasForeignKey(c => c.VehicleOwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompanyAgent>()
            .HasOne(a => a.VehicleOwner)
            .WithMany(v => v.Agents)
            .HasForeignKey(a => a.VehicleOwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CompanyAgent>()
            .HasOne(a => a.UserAccount)
            .WithMany(u => u.CompanyAgents)
            .HasForeignKey(a => a.UserAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<VehicleDetail>()
            .HasOne(v => v.VehicleOwner)
            .WithMany(v => v.Vehicles)
            .HasForeignKey(v => v.VehicleOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VehicleDetail>()
            .HasOne(v => v.Company)
            .WithMany(c => c.Vehicles)
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.VehicleOwner)
            .WithMany(v => v.Drivers)
            .HasForeignKey(d => d.VehicleOwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Driver>()
            .HasOne(d => d.UserAccount)
            .WithMany(u => u.Drivers)
            .HasForeignKey(d => d.UserAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Driver>()
            .HasIndex(d => d.IdNumber)
            .IsUnique();

        modelBuilder.Entity<GoodsOwner>()
            .HasOne(g => g.UserAccount)
            .WithMany(u => u.GoodsOwners)
            .HasForeignKey(g => g.UserAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Cargo>()
            .HasOne(c => c.GoodsOwner)
            .WithMany(g => g.Cargoes)
            .HasForeignKey(c => c.GoodsOwnerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CargoBid>()
            .HasOne(b => b.Cargo)
            .WithMany(c => c.Bids)
            .HasForeignKey(b => b.CargoId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CargoBid>()
            .HasOne(b => b.CompanyAgent)
            .WithMany(a => a.CargoBids)
            .HasForeignKey(b => b.CompanyAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CargoBid>()
            .HasOne(b => b.Driver)
            .WithMany(d => d.Bids)
            .HasForeignKey(b => b.DriverId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CargoBid>()
            .HasOne(b => b.VehicleRoute)
            .WithMany(r => r.CargoBids)
            .HasForeignKey(b => b.VehicleRouteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CargoBid>()
            .HasOne(b => b.PriceDecidedByGoodsOwner)
            .WithMany(g => g.PriceDecisions)
            .HasForeignKey(b => b.PriceDecidedByGoodsOwnerId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<CargoBid>()
            .HasIndex(b => new { b.CargoId, b.CompanyAgentId })
            .IsUnique();

        modelBuilder.Entity<CargoBid>()
            .HasIndex(b => new { b.Status, b.CreatedAt });

        modelBuilder.Entity<VehicleRoute>()
            .HasOne(r => r.VehicleDetail)
            .WithMany(v => v.Routes)
            .HasForeignKey(r => r.VehicleDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VehicleRoute>()
            .HasOne(r => r.CompanyAgent)
            .WithMany(a => a.VehicleRoutes)
            .HasForeignKey(r => r.CompanyAgentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VehicleRoute>()
            .HasIndex(r => new { r.IsAwaitingReturn, r.ReturnAvailabilityStartsAt, r.ReturnAvailabilityEndsAt });

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.RoutePathJson)
            .HasMaxLength(1_000_000);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.MapProvider)
            .HasMaxLength(100);

        modelBuilder.Entity<DeliveryReceipt>()
            .HasOne(r => r.CargoBid)
            .WithOne(b => b.DeliveryReceipt)
            .HasForeignKey<DeliveryReceipt>(r => r.CargoBidId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DeliveryReceipt>()
            .HasOne(r => r.Driver)
            .WithMany(d => d.UploadedDeliveryReceipts)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DeliveryReceipt>()
            .HasOne(r => r.ConfirmedByCompanyAgent)
            .WithMany(a => a.ConfirmedDeliveryReceipts)
            .HasForeignKey(r => r.ConfirmedByCompanyAgentId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<DeliveryReceipt>()
            .HasIndex(r => r.ConfirmationStatus);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.UserAccount)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasOne(n => n.VehicleOwner)
            .WithMany(v => v.Notifications)
            .HasForeignKey(n => n.VehicleOwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserAccountId, n.IsRead, n.CreatedAt });

        modelBuilder.Entity<Notification>()
            .ToTable(table => table.HasCheckConstraint(
                "CK_Notifications_ExactlyOneRecipient",
                "(([UserAccountId] IS NOT NULL AND [VehicleOwnerId] IS NULL) OR ([UserAccountId] IS NULL AND [VehicleOwnerId] IS NOT NULL))"));

        modelBuilder.Entity<Cargo>()
            .ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_Cargo_FromCoordinates",
                    "([FromLatitude] IS NULL OR [FromLatitude] BETWEEN -90 AND 90) AND ([FromLongitude] IS NULL OR [FromLongitude] BETWEEN -180 AND 180)");
                table.HasCheckConstraint(
                    "CK_Cargo_ToCoordinates",
                    "([ToLatitude] IS NULL OR [ToLatitude] BETWEEN -90 AND 90) AND ([ToLongitude] IS NULL OR [ToLongitude] BETWEEN -180 AND 180)");
                table.HasCheckConstraint(
                    "CK_Cargo_PricingMode",
                    "([PricingMode] = 'FixedTotal' AND [PricePerKg] IS NULL) OR ([PricingMode] = 'PerKg' AND [WeightKg] > 0 AND [PricePerKg] > 0)");
                table.HasCheckConstraint(
                    "CK_Cargo_ScheduledDates",
                    "[RequiredDepartureAt] IS NULL OR [DeliveryDeadlineAt] IS NULL OR [DeliveryDeadlineAt] >= [RequiredDepartureAt]");
                table.HasCheckConstraint(
                    "CK_Cargo_ActualDates",
                    "[DepartedAt] IS NULL OR [DeliveredAt] IS NULL OR [DeliveredAt] >= [DepartedAt]");
                table.HasCheckConstraint(
                    "CK_Cargo_RetentionExpiry",
                    "[RetentionExpiresAt] IS NULL OR [RequiredDepartureAt] IS NULL OR [RetentionExpiresAt] >= [RequiredDepartureAt]");
            });

        modelBuilder.Entity<Cargo>()
            .HasIndex(c => new { c.Status, c.RetentionExpiresAt });

        modelBuilder.Entity<VehicleRoute>()
            .ToTable(table =>
            {
                table.HasCheckConstraint(
                    "CK_VehicleRoutes_FromCoordinates",
                    "([FromLatitude] IS NULL OR [FromLatitude] BETWEEN -90 AND 90) AND ([FromLongitude] IS NULL OR [FromLongitude] BETWEEN -180 AND 180)");
                table.HasCheckConstraint(
                    "CK_VehicleRoutes_ToCoordinates",
                    "([ToLatitude] IS NULL OR [ToLatitude] BETWEEN -90 AND 90) AND ([ToLongitude] IS NULL OR [ToLongitude] BETWEEN -180 AND 180)");
                table.HasCheckConstraint(
                    "CK_VehicleRoutes_TripTimes",
                    "[ExpectedArrivalAt] >= [TripStartsAt]");
                table.HasCheckConstraint(
                    "CK_VehicleRoutes_ReturnWindow",
                    "[ReturnAvailabilityEndsAt] IS NULL OR [ReturnAvailabilityStartsAt] IS NULL OR [ReturnAvailabilityEndsAt] >= [ReturnAvailabilityStartsAt]");
            });

        modelBuilder.Entity<Cargo>()
            .Property(c => c.FromLocation)
            .HasMaxLength(300);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.ToLocation)
            .HasMaxLength(300);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.FromPlaceId)
            .HasMaxLength(200);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.ToPlaceId)
            .HasMaxLength(200);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.FromLocation)
            .HasMaxLength(300);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.ToLocation)
            .HasMaxLength(300);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.FromPlaceId)
            .HasMaxLength(200);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.ToPlaceId)
            .HasMaxLength(200);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.WeightKg)
            .HasPrecision(12, 3);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.TransportCost)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.PricePerKg)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.CommissionPercent)
            .HasPrecision(5, 2);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.AdvancePaymentPercent)
            .HasPrecision(5, 2);

        modelBuilder.Entity<CargoBid>()
            .Property(b => b.BidAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<CargoBid>()
            .Property(b => b.SuggestedAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.FromLatitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.FromLongitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.ToLatitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<Cargo>()
            .Property(c => c.ToLongitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.FromLatitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.FromLongitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.ToLatitude)
            .HasPrecision(9, 6);

        modelBuilder.Entity<VehicleRoute>()
            .Property(r => r.ToLongitude)
            .HasPrecision(9, 6);

        base.OnModelCreating(modelBuilder);
    }
}