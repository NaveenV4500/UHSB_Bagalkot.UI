using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.UI.Models;

public partial class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime Expires { get; set; }

    public DateTime Created { get; set; }

    public DateTime? Revoked { get; set; }

    public virtual UserMaster User { get; set; } = null!;
}
