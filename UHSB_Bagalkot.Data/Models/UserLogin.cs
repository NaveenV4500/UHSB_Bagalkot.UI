using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UserLogin
{
    public string LoginProvider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public string? ProviderDisplayName { get; set; }

    public int UserId { get; set; }

    public virtual UserMaster User { get; set; } = null!;
}
