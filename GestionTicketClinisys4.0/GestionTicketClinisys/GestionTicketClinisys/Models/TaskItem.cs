using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionTicketClinisys.Models.Enums;

namespace GestionTicketClinisys.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Enums.TaskStatus Status { get; set; } = Enums.TaskStatus.NotStarted;

        // Enhanced fields for Gantt and project management
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public int Priority { get; set; } = 2; // 1=Low, 2=Medium, 3=High, 4=Critical
        public int EstimatedHours { get; set; } = 0;
        public int ActualHours { get; set; } = 0;
        public int Progress { get; set; } = 0; // 0-100%
        public string? Tags { get; set; } // JSON array of tags
        public string? Comments { get; set; } // JSON array of comments
        public int KanbanOrder { get; set; } = 0; // For drag-and-drop ordering

        // Foreign Keys
        [Required]
        public int TicketId { get; set; }
        [ForeignKey("TicketId")]
        public virtual Ticket? Ticket { get; set; }

        [Required]
        public int AssignedUserId { get; set; }
        [ForeignKey("AssignedUserId")]
        public virtual User? AssignedUser { get; set; }
    }
}
