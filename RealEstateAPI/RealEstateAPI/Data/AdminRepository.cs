using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RealEstateAPI.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace RealEstateAPI.Data
{
    public class AdminRepository
    {
        private readonly string _connectionString;

        public AdminRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region GetStats
        public AdminStatsModel GetAdminStats()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAdminStats", conn) { CommandType = CommandType.StoredProcedure };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new AdminStatsModel
                    {
                        TotalUsers = Convert.ToInt32(reader["TotalUsers"]),
                        ActiveProperties = Convert.ToInt32(reader["ActiveProperties"]),
                        PendingApprovals = Convert.ToInt32(reader["PendingApprovals"]),
                        TotalTransactions = Convert.ToInt32(reader["TotalTransactions"])
                    };
                }
                return new AdminStatsModel();
            }
        }
        #endregion

        #region GetRecentActivity
        public (List<ActivityItemModel> Properties, List<ActivityItemModel> Users) GetRecentActivity()
        {
            var recentProps = new List<ActivityItemModel>();
            var recentUsers = new List<ActivityItemModel>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetRecentActivity", conn) { CommandType = CommandType.StoredProcedure };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                // First result set = properties
                while (reader.Read())
                {
                    recentProps.Add(new ActivityItemModel
                    {
                        ActivityType = reader["ActivityType"].ToString(),
                        Name = reader["Name"].ToString(),
                        Detail = reader["Detail"].ToString(),
                        RefID = Convert.ToInt32(reader["RefID"])
                    });
                }

                // Second result set = users
                reader.NextResult();
                while (reader.Read())
                {
                    recentUsers.Add(new ActivityItemModel
                    {
                        ActivityType = reader["ActivityType"].ToString(),
                        Name = reader["Name"].ToString(),
                        Detail = reader["Detail"].ToString(),
                        RefID = Convert.ToInt32(reader["RefID"])
                    });
                }
            }
            return (recentProps, recentUsers);
        }
        #endregion

        #region GetAllUsers
        public IEnumerable<AdminUserModel> GetAllUsers()
        {
            var users = new List<AdminUserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllUsers", conn) { CommandType = CommandType.StoredProcedure };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(new AdminUserModel
                    {
                        UserID = Convert.ToInt32(reader["UserID"]),
                        UserName = reader["UserName"].ToString(),
                        Email = reader["Email"].ToString(),
                        PhoneNo = reader["PhoneNo"] != DBNull.Value ? reader["PhoneNo"].ToString() : "",
                        Role = reader["Role"].ToString()
                    });
                }
            }
            return users;
        }
        #endregion

        #region GetAllPropertiesAdmin
        public IEnumerable<AdminPropertyModel> GetAllPropertiesAdmin()
        {
            var props = new List<AdminPropertyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllPropertiesAdmin", conn) { CommandType = CommandType.StoredProcedure };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    props.Add(new AdminPropertyModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Title = reader["Title"].ToString(),
                        Location = reader["Location"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Status = reader["Status"].ToString(),
                        Type = reader["Type"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        IsApproved = Convert.ToBoolean(reader["IsApproved"]),
                        CreatedBy = reader["CreatedBy"].ToString(),
                        SellerPhone = reader["SellerPhone"] != DBNull.Value ? reader["SellerPhone"].ToString() : ""
                    });
                }
            }
            return props;
        }
        #endregion

        #region GetAllFeedback
        public IEnumerable<AdminFeedbackModel> GetAllFeedbackAdmin()
        {
            var feedbacks = new List<AdminFeedbackModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetAllFeedbackAdmin", conn) { CommandType = CommandType.StoredProcedure };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    feedbacks.Add(new AdminFeedbackModel
                    {
                        FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                        Description = reader["Description"].ToString(),
                        ReviewerName = reader["ReviewerName"].ToString(),
                        PropertyTitle = reader["PropertyTitle"].ToString(),
                        PropertyID = Convert.ToInt32(reader["PropertyID"])
                    });
                }
            }
            return feedbacks;
        }
        #endregion

        #region UpdateUserRole
        public bool UpdateUserRole(int userID, string newRole)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateUserRole", conn) { CommandType = CommandType.StoredProcedure };
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@NewRole", newRole);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion
    }
}
