using System;
using System.Collections.Generic;

namespace kando_backend.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int? FromUserId { get; set; }

    public int ToUserId { get; set; }

    public int? TeamId { get; set; }

    public string Type { get; set; } = null!;

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? TaskId { get; set; }

    public virtual User? FromUser { get; set; }

    public virtual Task? Task { get; set; }

    public virtual Team? Team { get; set; }

    public virtual User ToUser { get; set; } = null!;
}
