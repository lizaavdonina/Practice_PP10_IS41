public class Deposit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ContractNumber { get; set; }

        [Required]
        public int DepositorId { get; set; }

        [Required]
        public int DepositTypeId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime OpenDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CloseDate { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Активный";

        [Column(TypeName = "decimal(18,2)")]
        public decimal AccruedInterest { get; set; } = 0;

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        // Навигационные свойства
        [ForeignKey("DepositorId")]
        public Depositor Depositor { get; set; }

        [ForeignKey("DepositTypeId")]
        public DepositType DepositType { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<DepositAccrual> DepositAccruals { get; set; }
    }
