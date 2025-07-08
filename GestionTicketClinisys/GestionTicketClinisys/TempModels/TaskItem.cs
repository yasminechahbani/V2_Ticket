using System;
using System.Collections.Generic;

namespace GestionTicketClinisys.TempModels;

public partial class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int TicketId { get; set; }

    public int AssignedUserId { get; set; }

    public int Status { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User AssignedUser { get; set; } = null!;

    public virtual Ticket Ticket { get; set; } = null!;
}
