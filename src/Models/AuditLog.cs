using System;
using System.ComponentModel.DataAnnotations;

namespace SiteSB.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Пользователь")]
        public int? UserId { get; set; }

        [Required]
        [Display(Name = "Действие")]
        public string ?Action { get; set; }

        [Required]
        [Display(Name = "Тип сущности")]
        public string ?EntityType { get; set; }

        [Display(Name = "ID сущности")]
        public int? EntityId { get; set; }

        [Display(Name = "Старые значения")]
        public string ?OldValues { get; set; }

        [Display(Name = "Новые значения")]
        public string ?NewValues { get; set; }

        [Display(Name = "Время")]
        public DateTime Timestamp { get; set; }

        [Display(Name = "IP адрес")]
        public string ?IPAddress { get; set; }

        public virtual User ?User { get; set; }
    }
}
