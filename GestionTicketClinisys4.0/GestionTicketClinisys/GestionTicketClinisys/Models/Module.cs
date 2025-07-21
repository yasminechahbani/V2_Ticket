using System.ComponentModel.DataAnnotations;

namespace GestionTicketClinisys.Models
{
    public class Module
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        // Navigation properties
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
