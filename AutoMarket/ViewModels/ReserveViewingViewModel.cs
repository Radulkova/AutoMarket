using System.ComponentModel.DataAnnotations;

namespace AutoMarket.ViewModels
{
    public class ReserveViewingViewModel
    {
        [Required]
        public int CarId { get; set; }

        [Required(ErrorMessage = "Въведи име.")]
        [StringLength(30)]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Въведи фамилия.")]
        [StringLength(30)]
        public string LastName { get; set; } = "";

        [Required(ErrorMessage = "Въведи телефон.")]
        [Phone]
        [StringLength(30)]
        public string Phone { get; set; } = "";

        [StringLength(200)]
        public string? Note { get; set; }
    }
}