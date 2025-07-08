using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionTicketClinisys.Models.Enums;

namespace GestionTicketClinisys.Models
{
    public class TaskItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public Ticket Ticket { get; set; } = null!;

        public int AssignedUserId { get; set; }
        [ForeignKey("AssignedUserId")]
        public User AssignedUser { get; set; } = null!;

        public TicketStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}