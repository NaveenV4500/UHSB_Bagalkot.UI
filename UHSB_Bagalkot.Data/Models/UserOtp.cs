using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UserOtp
{
    public int OtpId { get; set; }

    public int UserId { get; set; }

    public int Otp { get; set; }

    public DateTime ExpiryTime { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual UserMaster User { get; set; } = null!;
}
