using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteSB.Models
{
public class ClientCategory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        [StringLength(500)]
        public string Benefits { get; set; }

        // Навигационные свойства
        public ICollection<Depositor> Depositors { get; set; }
    }
