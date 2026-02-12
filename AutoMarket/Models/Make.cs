using System.ComponentModel.DataAnnotations;

namespace AutoMarket.Models
{
    public class Make
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = null!;

        public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
    }
}

