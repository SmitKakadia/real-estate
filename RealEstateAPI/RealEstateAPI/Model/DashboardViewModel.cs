namespace RealEstateAPI.Model
{
    public class DashboardViewModel
    {
        public int TotalProperties { get; set; }
        public int TotalUsers { get; set; }

        public List<UserModel> RecentUsers { get; set; }
    }
}
