using System;
using System.ComponentModel.DataAnnotations;

namespace SiteSB.Models
{
public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }

        [Required]
        [StringLength(50)]
        public string EntityType { get; set; }

        public int? EntityId { get; set; }

        public string OldValues { get; set; }

        public string NewValues { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime Timestamp { get; set; }

        [StringLength(50)]
        public string IPAddress { get; set; }

        // Навигационные свойства
        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
