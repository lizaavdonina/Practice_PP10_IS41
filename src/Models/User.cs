public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLogin { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedDate { get; set; }

        // Навигационные свойства
        [ForeignKey("RoleId")]
        public Role Role { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<AuditLog> AuditLogs { get; set; }
    }
