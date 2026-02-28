using System;
using System.Collections.Generic;

namespace kando_backend.Models;

public partial class Task
{
    public int Id { get; set; }

    /// <summary>
    /// Pertenece a una columna del tablero (ej: To Do)
    /// </summary>
    public int ListId { get; set; }

    public int CreatorId { get; set; }

    public int? AssignedToId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Icon { get; set; }

    public string Priority { get; set; } = null!;

    public string? Labels { get; set; }

    public DateTime? DueDate { get; set; }

    public bool? IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? AssignedTo { get; set; }

    public virtual User Creator { get; set; } = null!;

    public virtual BoardList List { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
