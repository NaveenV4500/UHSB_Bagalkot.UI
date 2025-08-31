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

    public virtual ICollection<ArticleItem> ArticleItems { get; set; } = new List<ArticleItem>();

    public virtual ICollection<Article> Articles { get; set; } = new List<Article>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
