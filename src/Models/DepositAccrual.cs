using System;
using System.ComponentModel.DataAnnotations;

namespace SiteSB.Models
{
    public class DepositAccrual
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DepositId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AccrualDate { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Навигационные свойства
        [ForeignKey("DepositId")]
        public Deposit Deposit { get; set; }
    }
