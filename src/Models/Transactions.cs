using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SiteSB.Models
{
    [Table("Transactions")]
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DepositId { get; set; }

        [Required]
        [StringLength(50)]
        public string TransactionType { get; set; } // 'Пополнение', 'Снятие', 'Начисление процентов'

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime TransactionDate { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int? PerformedBy { get; set; }

        // Навигационные свойства
        [ForeignKey("DepositId")]
        public Deposit Deposit { get; set; }

        [ForeignKey("PerformedBy")]
        public User User { get; set; }
    }
