using System;
using System.Collections.Generic;

namespace GestionTicketClinisys.TempModels;

public partial class Module
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
