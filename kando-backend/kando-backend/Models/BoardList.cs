using System;
using System.Collections.Generic;

namespace kando_backend.Models;

public partial class BoardList
{
    public int Id { get; set; }

    public int BoardId { get; set; }

    public string Name { get; set; } = null!;

    public int? Position { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual Board Board { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
