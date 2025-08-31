using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class Crop
{
    public int CropId { get; set; }

    public int CategoryId { get; set; }

    public string CropName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public virtual ICollection<ArticleItem> ArticleItems { get; set; } = new List<ArticleItem>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual CropCategory Category { get; set; } = null!;

    public virtual ICollection<CropDetail> CropDetails { get; set; } = new List<CropDetail>();
}
