using eTickets.Data.Base;
using System.ComponentModel.DataAnnotations;

namespace eTickets.Models
{
    public class Producer:IEntityBase
    {

        [Key]
        public int Id { get; set; }

        [Display(Name ="Profile Picture")]
        [Required(ErrorMessage = "Profile Picture is required")]
        public string profilepictureURl { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Full Name must be between 3 and 50 chars")]
        public string fullName { get; set; }

        [Display(Name = "Bio")]
        [Required(ErrorMessage = "Biography is required")]
        public string Bio { get; set; }

        //realtionships

        public List<Movie> Movies { get; set; } 

    }
}
