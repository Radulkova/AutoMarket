using System.ComponentModel.DataAnnotations;

namespace AutoMarket.Models
{
    public class CarModel
    {
        public int Id { get; set; }

        [Required, MaxLength(60)]
        public string Name { get; set; } = null!;

        public int MakeId { get; set; }
        public Make Make { get; set; } = null!;

        public ICollection<Car> Cars { get; set; } = new List<Car>();
    }
}

