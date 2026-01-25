using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteSB.Models
{
public class Depositor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(11)]
        public string INN { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(100)]
        public string Passport { get; set; }

        [Required]
        public int ClientCategoryId { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime RegistrationDate { get; set; }

        // Навигационные свойства
        [ForeignKey("ClientCategoryId")]
        public ClientCategory ClientCategory { get; set; }

        public ICollection<Deposit> Deposits { get; set; }
    }
