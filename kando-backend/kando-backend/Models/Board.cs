using System;
using System.Collections.Generic;

namespace kando_backend.Models;

public partial class Board
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Icon { get; set; }

    public int TeamId { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int TotalTasks { get; set; }

    public int CompletedTasks { get; set; }

    public virtual ICollection<BoardList> BoardLists { get; set; } = new List<BoardList>();

    public virtual Team Team { get; set; } = null!;
}
