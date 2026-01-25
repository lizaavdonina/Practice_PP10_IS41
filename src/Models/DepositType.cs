using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteSB.Models
{
public class DepositType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal InterestRate { get; set; }

        [Required]
        public int TermMonths { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal MinAmount { get; set; }

        public bool CanReplenish { get; set; } = false;

        public bool CanWithdraw { get; set; } = false;

        [StringLength(500)]
        public string Description { get; set; }

        // Навигационные свойства
        public ICollection<Deposit> Deposits { get; set; }
    }
