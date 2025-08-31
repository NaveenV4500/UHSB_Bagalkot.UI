using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class ArticleItem
{
    public int ArticleItemsId { get; set; }

    public int? CropId { get; set; }

    public int? ArticleId { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? ImageUrl { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Article? Article { get; set; }

    public virtual UserMaster? CreatedByNavigation { get; set; }

    public virtual Crop? Crop { get; set; }
}
