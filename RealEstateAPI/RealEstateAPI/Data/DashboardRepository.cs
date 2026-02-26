using RealEstateAPI.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using RealEstateAPI.Model;  

namespace RealEstateAPI.Data
{
    public class DashboardRepository
    {
        private readonly string _connectionString;

        public DashboardRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public int GetTotalProperties()
        {
            int totalProperties = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetTotalProperties", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                   
                    totalProperties = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return totalProperties;
        }

        public int GetTotalUsers()
        {
            int totalUsers = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetTotalUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
            return totalUsers;
        }

        public List<UserModel> GetRecentlyRegisteredUsers()
        {
            var users = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetRecentlyRegisteredUsers", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    using (SqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            users.Add(new UserModel
                            {
                                UserID = Convert.ToInt32(rdr["UserID"]),
                                UserName = rdr["UserName"].ToString(),
                                Email = rdr["Email"].ToString(),
                               
                            });
                        }
                    }
                }
            }
            return users;
        }

        public DashboardViewModel GetDashboardData()
        {
            return new DashboardViewModel
            {
                TotalProperties = GetTotalProperties(),
                TotalUsers = GetTotalUsers(),
                RecentUsers = GetRecentlyRegisteredUsers()
            };
        }
    }
}
