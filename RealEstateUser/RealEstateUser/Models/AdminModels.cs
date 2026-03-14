using System.Collections.Generic;

namespace RealEstateUser.Models
{
    public class AdminStatsModel
    {
        public int TotalUsers { get; set; }
        public int ActiveProperties { get; set; }
        public int PendingApprovals { get; set; }
        public int TotalTransactions { get; set; }
    }

    public class ActivityItemModel
    {
        public string ActivityType { get; set; }
        public string Name { get; set; }
        public string Detail { get; set; }
        public int RefID { get; set; }
    }

    public class AdminUserModel
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Role { get; set; }
    }

    public class AdminPropertyModel
    {
        public int PropertyID { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public bool IsApproved { get; set; }
        public string CreatedBy { get; set; }
        public string SellerPhone { get; set; }
    }

    public class AdminFeedbackModel
    {
        public int FeedbackID { get; set; }
        public string Description { get; set; }
        public string ReviewerName { get; set; }
        public string PropertyTitle { get; set; }
        public int PropertyID { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public AdminStatsModel Stats { get; set; } = new AdminStatsModel();
        public List<ActivityItemModel> RecentProperties { get; set; } = new List<ActivityItemModel>();
        public List<ActivityItemModel> RecentUsers { get; set; } = new List<ActivityItemModel>();
        public List<PropertyModel> PendingProperties { get; set; } = new List<PropertyModel>();
        public List<AdminUserModel> AllUsers { get; set; } = new List<AdminUserModel>();
        public List<AdminPropertyModel> AllProperties { get; set; } = new List<AdminPropertyModel>();
        public List<AdminFeedbackModel> AllFeedback { get; set; } = new List<AdminFeedbackModel>();
    }
}
