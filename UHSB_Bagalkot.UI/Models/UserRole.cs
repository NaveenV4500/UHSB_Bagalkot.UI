using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.UI.Models;

public partial class UserRole
{
    public int Id { get; set; }

    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
