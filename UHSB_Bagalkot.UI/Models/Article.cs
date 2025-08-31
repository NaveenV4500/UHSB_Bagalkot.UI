using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.UI.Models;

public partial class Article
{
    public int ArticleId { get; set; }

    public int? CropId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? ImageUrl { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? CropDetailId { get; set; }

    public virtual ICollection<ArticleItem> ArticleItems { get; set; } = new List<ArticleItem>();

    public virtual UserMaster? CreatedByNavigation { get; set; }

    public virtual Crop? Crop { get; set; }

    public virtual CropDetail? CropDetail { get; set; }
}
