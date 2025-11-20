using System;
using System.Collections.Generic;

namespace UHSB_Bagalkot.Data.Models;

public partial class UhsbImageFile
{
    public int FileId { get; set; }

    public int? ItemId { get; set; }

    public int? FileType { get; set; }

    public string? FilePath { get; set; }
}
