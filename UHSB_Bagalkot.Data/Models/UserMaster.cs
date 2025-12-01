using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UserMaster
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? PasswordHash { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? RoleType { get; set; }

    public byte? DistrictsId { get; set; }

    public string? Village { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public byte? ModifiedBy { get; set; }

    public byte? CreatedBy { get; set; }

    public string? Address { get; set; }

    public decimal? LandSize { get; set; }

    public int? EmployeeId { get; set; }

    public string? EmailId { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<UhsbWeatherCastFileDetail> UhsbWeatherCastFileDetails { get; set; } = new List<UhsbWeatherCastFileDetail>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    public virtual ICollection<UserOtp> UserOtps { get; set; } = new List<UserOtp>();
}
