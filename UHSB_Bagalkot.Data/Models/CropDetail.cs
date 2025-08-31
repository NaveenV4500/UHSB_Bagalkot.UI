using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class CropDetail
{
    public int DetailId { get; set; }

    public int CropId { get; set; }

    public string DetailType { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual Crop Crop { get; set; } = null!;
}
