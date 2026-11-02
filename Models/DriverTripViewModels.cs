using System.ComponentModel.DataAnnotations;

namespace Return_trip.Models;

public sealed class DriverDashboardViewModel
{
    public string DriverName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string AssignedVehicle { get; set; } = string.Empty;
    public string DriverStatus { get; set; } = string.Empty;
    public string? LastCargoTransported { get; set; }
    public DateTime? LastDeliveryAt { get; set; }
    public string? CurrentActiveTrip { get; set; }
    public int TotalTrips { get; set; }
    public int ActiveTrips { get; set; }
    public int AwaitingPickup { get; set; }
    public int InTransit { get; set; }
    public int AwaitingPayment { get; set; }
    public int CompletedTrips { get; set; }
    public List<DriverTripViewModel> Trips { get; set; } = [];
}

public sealed class DriverTripViewModel
{
    public int BidId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string CargoTitle { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string VehicleRegistration { get; set; } = string.Empty;
    public decimal TripCost { get; set; }
    public decimal Deposit { get; set; }
    public decimal FuelCredit { get; set; }
    public decimal FinalBalance { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string TripStatus { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public string? ReceiverName { get; set; }
    public string? ReceiverPhone { get; set; }
    public List<TripActivity> Activities { get; set; } = [];
}

public sealed class SubmitFinalPaymentViewModel
{
    public int BidId { get; set; }

    [Required, StringLength(100)]
    public string Reference { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Amount { get; set; }
}

public sealed class RecordFuelAdvanceViewModel
{
    public int BidId { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Amount { get; set; }

    [Required, StringLength(100)]
    public string Reference { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime PaidAt { get; set; }
}

public sealed class SubmitDeliveryEvidenceViewModel
{
    public int BidId { get; set; }

    [Required, StringLength(200)]
    public string ReceiverName { get; set; } = string.Empty;

    [Required]
    public IFormFile? SignedDocument { get; set; }
}

public sealed class AgentTripsViewModel
{
    public List<AgentTripRowViewModel> Trips { get; set; } = [];
}

public sealed class AgentTripRowViewModel
{
    public int BidId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string CargoTitle { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public int? DriverId { get; set; }
    public string TripStatus { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal TripCost { get; set; }
    public decimal Deposit { get; set; }
    public decimal ReconciledFuel { get; set; }
    public decimal ConfirmedFinalPayments { get; set; }
    public decimal Outstanding { get; set; }
    public int? DeliveryReceiptId { get; set; }
    public string? DeliveryEvidenceName { get; set; }
    public string? ReceiptStatus { get; set; }
    public List<AgentDriverOptionViewModel> Drivers { get; set; } = [];
    public List<CargoPayment> PendingPayments { get; set; } = [];
    public List<FuelAdvance> FuelAdvances { get; set; } = [];
}

public sealed class AgentDriverOptionViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class VerifyPaymentViewModel
{
    public int PaymentId { get; set; }

    [StringLength(500)]
    public string? Comments { get; set; }
}

public sealed class AddFuelAdvanceViewModel
{
    public int BidId { get; set; }

    [Range(typeof(decimal), "0.01", "9999999999999999")]
    public decimal Amount { get; set; }

    [Required, StringLength(100)]
    public string Reference { get; set; } = string.Empty;

    [DataType(DataType.DateTime)]
    public DateTime PaidAt { get; set; } = DateTime.UtcNow;
}

public sealed class AssignDriverViewModel
{
    public int BidId { get; set; }

    [Range(1, int.MaxValue)]
    public int DriverId { get; set; }
}
