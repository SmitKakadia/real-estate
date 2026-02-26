using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RealEstateAPI.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace RealEstateAPI.Data
{
    public class UserRepository
    {
        private readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAllUsers
        public IEnumerable<UserModel> SelectAllUser()
        {
            var users = new List<UserModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectAllUsers", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    users.Add(ReadUser(reader));
                }
            }
            return users;
        }
        #endregion

        #region SelectUserByID
        public UserModel? SelectUserByID(int userID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectUserByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return ReadUser(reader);
                }
            }
            return null;
        }
        #endregion

        #region GetUserByEmail (For Registration Check)
        public UserModel? GetUserByEmail(string email)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetUserByEmail", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Email", email);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return ReadUser(reader);
                }
            }
            return null;
        }
        #endregion

        #region LoginUser (Using Plain Password Comparison)
        public UserModel? LoginUser(string email, string password)
        {
            var user = GetUserByEmail(email);
            if (user == null)
                return null; 

            
            if (user.Password != password)
                return null; 

            return user; 
        }
        #endregion

        #region InsertUser (Store Plain Text Password)
        public bool InsertUser(UserModel user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Password", user.Password); // Store as plain text
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);
                cmd.Parameters.AddWithValue("@Role", user.Role);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region UpdateUser (Exclude Password Update)
        public bool UpdateUser(UserModel user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", user.UserID);
                cmd.Parameters.AddWithValue("@UserName", user.UserName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@PhoneNo", user.PhoneNo);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region DeleteUser
        public bool DeleteUser(int userID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteUser", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
        #endregion

        #region ReadUser Helper Function
        private UserModel ReadUser(SqlDataReader reader)
        {
            return new UserModel
            {
                UserID = Convert.ToInt32(reader["UserID"]),
                UserName = reader["UserName"].ToString(),
                Email = reader["Email"].ToString(),
                Password = reader["Password"].ToString(),
                PhoneNo = reader["PhoneNo"].ToString(),
                Role = reader["Role"].ToString()
            };
        }
        #endregion
    }
}
