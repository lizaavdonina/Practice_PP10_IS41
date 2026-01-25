using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SiteSB.Models
{
    public class ClientCategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Название категории")]
        public string ?Name { get; set; }

        [Display(Name = "Описание")]
        public string ?Description { get; set; }

        [Display(Name = "Преимущества")]
        public string ?Benefits { get; set; }

        public virtual ICollection<Depositor> ?Depositors { get; set; }
    }
}
