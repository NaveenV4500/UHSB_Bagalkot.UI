using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbCartMaster
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public virtual ICollection<UhsbCartItem> UhsbCartItems { get; set; } = new List<UhsbCartItem>();

    public virtual UserMaster User { get; set; } = null!;
}
