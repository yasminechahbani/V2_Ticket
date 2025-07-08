using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GestionTicketClinisys.Models.Enums;

namespace GestionTicketClinisys.Models
{
    public class Ticket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        public TicketStatus Status { get; set; }

        public TicketPriority Priority { get; set; }

        // New fields for enhanced ticket management
        public int? DurationDays { get; set; }

        public int? DurationMonths { get; set; }

        public TicketType? Type { get; set; }

        public string? RichTextComment { get; set; }

        public string? AttachedFiles { get; set; } // JSON string to store file paths/info

        public int ClientId { get; set; }
        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int? TeamId { get; set; }
        [ForeignKey("TeamId")]
        public virtual Team? Team { get; set; }

        public int? ModuleId { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module? Module { get; set; }

        public virtual ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
