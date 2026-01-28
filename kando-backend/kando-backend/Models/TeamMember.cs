using System;
using System.Collections.Generic;

namespace kando_backend.Models;

public partial class TeamMember
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int TeamId { get; set; }

    public string Role { get; set; } = null!;

    public string Status { get; set; } = null!;

    public DateTime? JoinedAt { get; set; }

    public DateTime? RemovedAt { get; set; }

    public virtual Team Team { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
