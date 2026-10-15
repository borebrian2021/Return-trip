# Return Trip — Database Structure

> Last updated to reflect current project state (October 2026).

---

## Tables Overview

| Table | EF DbSet | Description |
|---|---|---|
| `UserAccounts` | `UserAccounts` | ASP.NET Identity user (extends `IdentityUser<int>`) |
| `VehicleOwners` | `VehicleOwners` | Transport business owner profile |
| `CompanyRegistrations` | `CompanyRegistrations` | Companies registered under a VehicleOwner |
| `CompanyAgents` | `CompanyAgents` | Agents working for a VehicleOwner/Company |
| `VehicleDetails` | `VehicleDetails` | Vehicles registered under a Company |
| `VehicleRoutes` | `VehicleRoutes` | Planned trip a vehicle is making (with return window) |
| `Drivers` | `Drivers` | Drivers employed under a VehicleOwner |
| `GoodsOwners` | `GoodsOwners` | Cargo shippers / clients |
| `Cargoes` | `Cargoes` | Cargo listings posted for bidding |
| `CargoBids` | `CargoBids` | Bids placed by Agents on Cargo listings |
| `DeliveryReceipts` | `DeliveryReceipts` | Signed receipt uploaded by Driver, confirmed by Agent |
| `BankDetails` | `BankDetails` | Bank account details (VehicleOwner or GoodsOwner) |
| `Notifications` | `Notifications` | In-app notifications (UserAccount or VehicleOwner) |
| `AdminCommissionSettings` | `AdminCommissionSettings` | Global admin commission configuration |

Plus the standard ASP.NET Identity tables: `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`, `AspNetRoleClaims`.

---

## Roles (AppRoles.cs)

| Constant | Value | Description |
|---|---|---|
| `AppRoles.Administrator` | `"Administrator"` | Platform admin |
| `AppRoles.GoodsOwner` | `"GoodsOwner"` | Posts cargo listings |
| `AppRoles.VehicleOwner` | `"VehicleOwner"` | Manages fleet, drivers, and agents |
| `AppRoles.Driver` | `"Driver"` | Assigned to trips, uploads delivery receipts |
| `AppRoles.CompanyAgent` | `"CompanyAgent"` | Bids on cargo, manages vehicle routes, confirms deliveries |

---

## Table Definitions

### UserAccounts

Extends `IdentityUser<int>`. Stores first name, last name, and approval status alongside the standard Identity columns (email, password hash, phone, etc.).

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `FirstName` | `string` | | |
| `LastName` | `string` | | |
| `IsApproved` | `bool` | | Admin approval flag |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |
| *(Identity columns)* | | | `UserName`, `NormalizedEmail` (unique index), `PasswordHash`, `PhoneNumber`, etc. |

**Referenced by:** `VehicleOwners`, `CompanyAgents` (nullable), `Drivers` (nullable), `GoodsOwners`, `Notifications` (nullable)

---

### VehicleOwners

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `UserAccountId` | `int` | FK → `UserAccounts` (Restrict) | |
| `CompanyName` | `string` | | |
| `BusinessRegistrationNumber` | `string` | | |
| `Email` | `string` | | |
| `Phone` | `string` | | |
| `Location` | `string` | | |
| `IsApproved` | `bool` | | |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

**Referenced by:** `CompanyRegistrations`, `CompanyAgents`, `VehicleDetails`, `Drivers`, `BankDetails`, `Notifications` (nullable)

---

### CompanyRegistrations

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `VehicleOwnerId` | `int` | FK → `VehicleOwners` (Cascade) | |
| `CompanyName` | `string` | | |
| `RegistrationNumber` | `string` | | |
| `Email` | `string` | | |
| `Phone` | `string` | | |
| `Location` | `string` | | |
| `NumberPlate` | `string` | | |
| `CertificateOfRegistrationUrl` | `string` | | File URL |
| `CertificateOfRegistrationType` | `string` | | File MIME type / extension |
| `IsApproved` | `bool` | | |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

**Referenced by:** `VehicleDetails`, `CompanyAgents` (nullable)

---

### CompanyAgents

Agents act on behalf of a VehicleOwner. They bid on cargo and manage vehicle routes.

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `UserAccountId` | `int?` | FK → `UserAccounts` (SetNull) | Null until agent claims their profile |
| `VehicleOwnerId` | `int` | FK → `VehicleOwners` (Cascade) | |
| `CompanyId` | `int?` | FK → `CompanyRegistrations` (nullable) | Optional company assignment |
| `FullName` | `string` | | |
| `Email` | `string` | | |
| `PhoneNumber` | `string` | | |
| `IsActive` | `bool` | Default: `true` | |

**Referenced by:** `VehicleRoutes`, `CargoBids`, `DeliveryReceipts` (as confirming agent)

---

### VehicleDetails

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `VehicleOwnerId` | `int` | FK → `VehicleOwners` (Restrict) | |
| `CompanyId` | `int` | FK → `CompanyRegistrations` (Restrict) | |
| `VehicleType` | `string` | | |
| `RegistrationNumber` | `string` | | |
| `LicenseNumber` | `string` | | |
| `LicenseDocumentUrl` | `string` | | File URL |
| `LicenseDocumentType` | `string` | | File MIME type / extension |
| `IsApproved` | `bool` | | |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

**Referenced by:** `VehicleRoutes`

---

### VehicleRoutes

Represents a planned vehicle trip. Agents post routes to advertise a vehicle with available return capacity.

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `VehicleDetailId` | `int` | FK → `VehicleDetails` (Cascade) | |
| `CompanyAgentId` | `int` | FK → `CompanyAgents` (Restrict) | Agent who posted the route |
| `FromLocation` | `string` | Max 300 chars | |
| `FromPlaceId` | `string?` | Max 200 chars | Maps API place ID |
| `FromLatitude` | `decimal?` | Precision (9,6) | |
| `FromLongitude` | `decimal?` | Precision (9,6) | |
| `ToLocation` | `string` | Max 300 chars | |
| `ToPlaceId` | `string?` | Max 200 chars | Maps API place ID |
| `ToLatitude` | `decimal?` | Precision (9,6) | |
| `ToLongitude` | `decimal?` | Precision (9,6) | |
| `TripStartsAt` | `DateTime` | | |
| `ExpectedArrivalAt` | `DateTime` | CHECK: ≥ `TripStartsAt` | |
| `ReturnAvailabilityStartsAt` | `DateTime?` | | When return capacity opens |
| `ReturnAvailabilityEndsAt` | `DateTime?` | CHECK: ≥ `ReturnAvailabilityStartsAt` | |
| `IsAwaitingReturn` | `bool` | | |
| `RoutePathJson` | `string?` | Max 1,000,000 chars | GeoJSON LineString for corridor matching |
| `MapProvider` | `string?` | Max 100 chars | e.g. `"google"`, `"osm"` |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |
| `UpdatedAt` | `DateTime?` | | |

**Indexes:** `(IsAwaitingReturn, ReturnAvailabilityStartsAt, ReturnAvailabilityEndsAt)`

**Check constraints:**
- `CK_VehicleRoutes_FromCoordinates` — lat ∈ [-90,90], lng ∈ [-180,180]
- `CK_VehicleRoutes_ToCoordinates` — lat ∈ [-90,90], lng ∈ [-180,180]
- `CK_VehicleRoutes_TripTimes` — `ExpectedArrivalAt >= TripStartsAt`
- `CK_VehicleRoutes_ReturnWindow` — `ReturnAvailabilityEndsAt >= ReturnAvailabilityStartsAt`

**Referenced by:** `CargoBids`

---

### Drivers

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `UserAccountId` | `int?` | FK → `UserAccounts` (SetNull) | Null until driver claims their profile |
| `VehicleOwnerId` | `int` | FK → `VehicleOwners` (Cascade) | |
| `FullName` | `string` | | |
| `IdNumber` | `string` | Unique index | National ID number |
| `Email` | `string` | | |
| `PhoneNumber` | `string` | | |
| `LicenseNumber` | `string` | | |
| `LicenseImageUrl` | `string` | | File URL |
| `LicenseImageType` | `string` | | File MIME type / extension |
| `IsVerified` | `bool` | | Set to `true` when profile is claimed |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

**Referenced by:** `CargoBids` (nullable), `DeliveryReceipts`

---

### GoodsOwners

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `UserAccountId` | `int` | FK → `UserAccounts` (Restrict) | |
| `Name` | `string` | | Business name |
| `Email` | `string` | | |
| `Phone` | `string` | | |
| `Location` | `string` | | |
| `IsVerified` | `bool` | | |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

**Referenced by:** `Cargoes`, `BankDetails`, `CargoBids` (as `PriceDecidedByGoodsOwner`)

---

### Cargoes

Cargo shipment listings. Supports two pricing modes and optional geo-coordinates for route matching.

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `GoodsOwnerId` | `int` | FK → `GoodsOwners` (Cascade) | |
| `Title` | `string` | | |
| `Description` | `string` | | |
| `FromLocation` | `string` | Max 300 chars | |
| `FromPlaceId` | `string?` | Max 200 chars | Maps API place ID |
| `FromLatitude` | `decimal?` | Precision (9,6) | |
| `FromLongitude` | `decimal?` | Precision (9,6) | |
| `ToLocation` | `string` | Max 300 chars | |
| `ToPlaceId` | `string?` | Max 200 chars | Maps API place ID |
| `ToLatitude` | `decimal?` | Precision (9,6) | |
| `ToLongitude` | `decimal?` | Precision (9,6) | |
| `RequiredDepartureAt` | `DateTime?` | | Requested pickup time |
| `DeliveryDeadlineAt` | `DateTime?` | CHECK: ≥ `RequiredDepartureAt` | |
| `DepartedAt` | `DateTime?` | | Actual departure |
| `DeliveredAt` | `DateTime?` | CHECK: ≥ `DepartedAt` | Actual delivery |
| `RetentionExpiresAt` | `DateTime?` | CHECK: ≥ `RequiredDepartureAt` | Payment release date |
| `ReopenedAt` | `DateTime?` | | If listing was reopened |
| `WeightKg` | `decimal` | Precision (12,3) | |
| `PackageType` | `string` | | |
| `PricingMode` | `string` | Default: `"FixedTotal"` | `"FixedTotal"` or `"PerKg"` |
| `PricePerKg` | `decimal?` | Precision (18,2) | Only set when `PricingMode = "PerKg"` |
| `TransportCost` | `decimal` | Precision (18,2) | |
| `CommissionPercent` | `decimal` | Precision (5,2) | |
| `RetentionPeriodDays` | `int` | | |
| `AdvancePaymentPercent` | `decimal` | Default: `70` | % paid upfront to agent |
| `Status` | `string` | Default: `"Open"` | `"Open"`, `"Closed"`, etc. |
| `IsBidOpen` | `bool` | Default: `true` | |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

**Indexes:** `(Status, RetentionExpiresAt)`

**Check constraints:**
- `CK_Cargo_FromCoordinates` / `CK_Cargo_ToCoordinates` — coordinate range validation
- `CK_Cargo_PricingMode` — `FixedTotal` requires null `PricePerKg`; `PerKg` requires positive weight + price per kg
- `CK_Cargo_ScheduledDates` — `DeliveryDeadlineAt >= RequiredDepartureAt`
- `CK_Cargo_ActualDates` — `DeliveredAt >= DepartedAt`
- `CK_Cargo_RetentionExpiry` — `RetentionExpiresAt >= RequiredDepartureAt`

**Referenced by:** `CargoBids`

---

### CargoBids

A bid placed by a CompanyAgent on behalf of a vehicle route for a cargo listing.

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `CargoId` | `int` | FK → `Cargoes` (Cascade) | |
| `CompanyAgentId` | `int` | FK → `CompanyAgents` (Restrict) | Agent placing the bid |
| `VehicleRouteId` | `int` | FK → `VehicleRoutes` (Restrict) | The vehicle route being offered |
| `DriverId` | `int?` | FK → `Drivers` (SetNull) | Assigned driver (optional) |
| `PriceDecidedByGoodsOwnerId` | `int?` | FK → `GoodsOwners` (NoAction) | Who accepted/rejected a price suggestion |
| `BidAmount` | `decimal` | Precision (18,2) | Initial bid price |
| `SuggestedAmount` | `decimal?` | Precision (18,2) | Counter-offer from agent |
| `SuggestionComments` | `string?` | | Notes on price suggestion |
| `PriceSuggestionStatus` | `string` | Default: `"NotRequired"` | `"NotRequired"`, `"Pending"`, `"Accepted"`, `"Rejected"` |
| `PriceDecisionAt` | `DateTime?` | | When goods owner responded to suggestion |
| `EstimatedVehicleArrivalAt` | `DateTime?` | | Estimated pickup time |
| `Status` | `string` | Default: `"Pending"` | `"Pending"`, `"Accepted"`, `"Rejected"`, `"Completed"` |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |
| `AcceptedAt` | `DateTime?` | | |
| `CompletedAt` | `DateTime?` | | |

**Indexes:**
- Unique: `(CargoId, CompanyAgentId)` — one bid per agent per cargo listing
- Composite: `(Status, CreatedAt)` — for status-filtered queries

**Referenced by:** `DeliveryReceipts` (one-to-one)

---

### DeliveryReceipts

Signed proof of delivery uploaded by the Driver, then confirmed or rejected by the CompanyAgent.

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `CargoBidId` | `int` | FK → `CargoBids` (Cascade), one-to-one | |
| `DriverId` | `int` | FK → `Drivers` (Restrict) | Driver who uploaded the receipt |
| `ConfirmedByCompanyAgentId` | `int?` | FK → `CompanyAgents` (SetNull) | Agent who confirmed/rejected |
| `ReceiverName` | `string` | | Person who received the goods |
| `SignedAt` | `DateTime` | | When the receipt was signed |
| `ImageUrl` | `string` | | File URL |
| `ImageContentType` | `string` | | File MIME type |
| `ImageFileName` | `string?` | | Original file name |
| `ConfirmationStatus` | `string` | Default: `"Pending"` | `"Pending"`, `"Confirmed"`, `"Rejected"` |
| `ConfirmedAt` | `DateTime?` | | When agent confirmed/rejected |
| `AgentComments` | `string?` | | Agent confirmation notes |
| `UploadedAt` | `DateTime` | Default: `UtcNow` | |

**Indexes:** `(ConfirmationStatus)`

---

### BankDetails

Shared between VehicleOwners and GoodsOwners — exactly one nullable FK is populated per row.

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `VehicleOwnerId` | `int?` | FK → `VehicleOwners` (nullable) | Null if owned by GoodsOwner |
| `GoodsOwnerId` | `int?` | FK → `GoodsOwners` (nullable) | Null if owned by VehicleOwner |
| `BankName` | `string` | | |
| `AccountName` | `string` | | |
| `AccountNumber` | `string` | | |
| `AccountType` | `string` | | e.g. `"Savings"`, `"Cheque"` |
| `SwiftCode` | `string` | | |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

---

### Notifications

In-app notifications. The DB enforces that exactly one of `UserAccountId` or `VehicleOwnerId` is set (XOR check constraint).

| Column | Type | Constraints | Notes |
|---|---|---|---|
| `Id` | `int` | PK, Identity | |
| `UserAccountId` | `int?` | FK → `UserAccounts` (Restrict) | |
| `VehicleOwnerId` | `int?` | FK → `VehicleOwners` (Restrict) | |
| `Title` | `string` | | |
| `Message` | `string` | | |
| `Type` | `string` | | Notification category |
| `RelatedEntityType` | `string?` | | e.g. `"CargoBid"`, `"Cargo"` |
| `RelatedEntityId` | `int?` | | PK of the related entity |
| `IsRead` | `bool` | | |
| `CreatedAt` | `DateTime` | Default: `UtcNow` | |

**Indexes:** `(UserAccountId, IsRead, CreatedAt)`

**Check constraint:** `CK_Notifications_ExactlyOneRecipient` — exactly one of `UserAccountId` / `VehicleOwnerId` must be non-null.

---

### AdminCommissionSettings

Global platform commission rate. No foreign key relationships.

| Column | Type | Notes |
|---|---|---|
| `Id` | `int` | PK, Identity |
| `Percentage` | `decimal` | Commission rate |
| `UpdatedAt` | `DateTime` | Last updated (default: `UtcNow`) |

---

## Entity Relationship Diagram

```
UserAccounts (Identity)
  ├──< VehicleOwners
  │       ├──< CompanyRegistrations
  │       │       ├──< VehicleDetails ──< VehicleRoutes ──< CargoBids
  │       │       └──< CompanyAgents ──────────────────────────┤
  │       ├──< CompanyAgents                                   │
  │       ├──< Drivers ──────────────────────< CargoBids       │
  │       ├──< BankDetails                        │            │
  │       └──< Notifications                      │            │
  │                                               │         VehicleRoutes
  ├──< GoodsOwners                                │
  │       ├──< Cargoes ──────────────────< CargoBids
  │       │                                       │
  │       │                                       └──── DeliveryReceipts
  │       ├──< BankDetails
  │       └── (PriceDecisions via CargoBids.PriceDecidedByGoodsOwner)
  ├──< CompanyAgents
  ├──< Drivers
  └──< Notifications

AdminCommissionSettings  (standalone)
```

---

## Key Design Decisions

| Decision | Detail |
|---|---|
| **Identity integration** | `UserAccounts` extends `IdentityUser<int>`, so PK is `int` not `Guid`. `SystemDBContext` extends `IdentityDbContext<UserAccount, IdentityRole<int>, int>`. |
| **Two-phase registration** | Drivers and Agents are pre-registered by the VehicleOwner (profile only). They later "claim" the profile by matching their ID number or email, then set a password. `UserAccountId` is nullable on both; it gets set on claim. |
| **Bids are agent-to-cargo** | `CargoBid` links a `CompanyAgent` + `VehicleRoute` to a `Cargo`. Drivers are optionally assigned afterward. The unique constraint is `(CargoId, CompanyAgentId)`. |
| **Price negotiation workflow** | `CargoBid` has a full price negotiation sub-flow: `SuggestedAmount`, `PriceSuggestionStatus`, `PriceDecisionAt`, `PriceDecidedByGoodsOwnerId`. |
| **Geo-matching support** | Both `Cargo` and `VehicleRoute` store place IDs + lat/long, and `VehicleRoute` supports a full GeoJSON `RoutePathJson` for corridor-based route matching. |
| **Cascade vs Restrict** | VehicleOwner → Company/Agents/Drivers/Routes all cascade. UserAccount FKs use Restrict or SetNull to prevent accidentally deleting a user taking out profile records. |
| **Status fields are strings** | `Cargo.Status`, `CargoBid.Status`, `CargoBid.PriceSuggestionStatus`, `DeliveryReceipt.ConfirmationStatus` are plain `string` columns; no database enums. |
| **File storage is URL-based** | Documents and images are stored as URL strings + a separate MIME/extension column. No binary data in the DB. |
| **BankDetails dual-FK** | One table serves both `VehicleOwner` and `GoodsOwner` via two nullable FKs. Exactly one is set per row (not enforced by a DB constraint, only by application logic). |
| **Notification recipient XOR** | A DB CHECK constraint enforces that each notification has exactly one recipient — either a `UserAccount` or a `VehicleOwner`, never both, never neither. |
| **Pricing mode integrity** | A DB CHECK constraint on `Cargoes` enforces the `PricingMode` field is consistent with `PricePerKg` (null for `FixedTotal`, positive for `PerKg`). |
| **No model data annotations** | All column constraints (lengths, precision, indexes, FKs, check constraints) are configured exclusively through the Fluent API in `SystemDBContext.OnModelCreating`. |
