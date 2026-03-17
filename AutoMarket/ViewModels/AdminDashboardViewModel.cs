namespace AutoMarket.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalCars { get; set; }
        public int ActiveCars { get; set; }
        public int SoldCars { get; set; }
        public int DeletedCars { get; set; }

        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ApprovedOrders { get; set; }
        public int RejectedOrders { get; set; }

        public List<AdminCarRowViewModel> LatestCars { get; set; } = new();
        public List<AdminOrderRowViewModel> LatestOrders { get; set; } = new();
    }

    public class AdminCarRowViewModel
    {
        public int Id { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal Price { get; set; }
        public bool IsSold { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class AdminOrderRowViewModel
    {
        public int Id { get; set; }
        public string CarName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string StatusText { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
    }
}
