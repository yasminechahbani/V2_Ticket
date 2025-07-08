using System;
using System.Collections.Generic;

namespace GestionTicketClinisys.TempModels;

public partial class Ticket
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime CreationDate { get; set; }

    public int Status { get; set; }

    public int Priority { get; set; }

    public int ClientId { get; set; }

    public int? UserId { get; set; }

    public int? TeamId { get; set; }

    public int? ModuleId { get; set; }

    public string? AttachedFiles { get; set; }

    public string? Designation { get; set; }

    public int? DurationDays { get; set; }

    public int? DurationMonths { get; set; }

    public int? FaultType { get; set; }

    public int? ProviderType { get; set; }

    public string? RichTextComment { get; set; }

    public int? Type { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual Module? Module { get; set; }

    public virtual ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();

    public virtual Team? Team { get; set; }

    public virtual User? User { get; set; }
}
