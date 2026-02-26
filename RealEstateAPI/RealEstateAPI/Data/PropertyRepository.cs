using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using RealEstateAPI.Model;
using System;
using System.Collections.Generic;
using System.Data;

namespace RealEstateAPI.Data
{
    public class PropertyRepository
    {
        private readonly string _connectionString;

        public PropertyRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAllPropertiesBySold
        public IEnumerable<PropertyModel> SelectAllPropertys()
        {
            var properties = new List<PropertyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectAllProperties", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    properties.Add(new PropertyModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        Title = reader["Title"].ToString(),
                        Status = reader["Status"].ToString(),
                        Type = reader["Type"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        UserID = Convert.ToInt32(reader["UserID"])
                    });
                }
            }
            return properties;
        }
        #endregion

        #region SelectAllProperties
        public IEnumerable<PropertyModel> SelectAllProperty()
        {
            var properties = new List<PropertyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetPendingProperties", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    properties.Add(new PropertyModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        Title = reader["Title"].ToString(),
                        Status = reader["Status"].ToString(),
                        Type = reader["Type"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        UserID = Convert.ToInt32(reader["UserID"])
                    });
                }
            }
            return properties;
        }
        #endregion

        #region SelectPropertyByID
        public PropertyModel SelectPropertyByID(int PropertyID)
        {
            PropertyModel property = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectPropertyByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", PropertyID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    property = new PropertyModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        Title = reader["Title"].ToString(),
                        Status = reader["Status"].ToString(),
                        Type = reader["Type"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        UserID = Convert.ToInt32(reader["UserID"])
                    };
                }
                reader.Close();
                
                if (property != null)
                {
                    SqlCommand imgCmd = new SqlCommand("GetPropertyImages", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    imgCmd.Parameters.AddWithValue("@PropertyID", PropertyID);
                    SqlDataReader imgReader = imgCmd.ExecuteReader();
                    while (imgReader.Read())
                    {
                        property.AdditionalImages.Add(imgReader["ImageUrl"].ToString());
                    }
                    imgReader.Close();
                }
            }
            return property;
        }
        #endregion

        #region InsertProperty
        public bool InsertProperty(PropertyModel property)
        {
            int rowsAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@Title", property.Title);
                cmd.Parameters.AddWithValue("@Location", property.Location);
                cmd.Parameters.AddWithValue("@Description", property.Description);
                cmd.Parameters.AddWithValue("@Price", property.Price);
                cmd.Parameters.AddWithValue("@Type", property.Type);
                cmd.Parameters.AddWithValue("@Status", property.Status);
                cmd.Parameters.AddWithValue("@UserID", property.UserID);
                cmd.Parameters.AddWithValue("@ImageUrl", property.ImageUrl);

                conn.Open();
                var result = cmd.ExecuteScalar();
                if (result != null)
                {
                    property.PropertyID = Convert.ToInt32(result);
                    rowsAffected = 1;

                    // Insert additional images correctly
                    foreach (var additionalImage in property.AdditionalImages)
                    {
                        SqlCommand imgCmd = new SqlCommand("InsertPropertyImage", conn)
                        {
                            CommandType = CommandType.StoredProcedure
                        };
                        imgCmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                        imgCmd.Parameters.AddWithValue("@ImageUrl", additionalImage);
                        imgCmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    rowsAffected = 0;
                }
            }
            return rowsAffected > 0;
        }
        #endregion

        #region UpdateProperty
        public bool UpdateProperty(PropertyModel property)
        {
            int rowsAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", property.PropertyID);
                cmd.Parameters.AddWithValue("@Title", property.Title);
                cmd.Parameters.AddWithValue("@Location", property.Location);
                cmd.Parameters.AddWithValue("@Description", property.Description);
                cmd.Parameters.AddWithValue("@Price", property.Price);
                cmd.Parameters.AddWithValue("@Type", property.Type);
                cmd.Parameters.AddWithValue("@Status", property.Status);
                cmd.Parameters.AddWithValue("@UserID", property.UserID);
                cmd.Parameters.AddWithValue("@ImageUrl", property.ImageUrl);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected > 0;
        }
        #endregion

        #region DeleteProperty
        public bool DeleteProperty(int PropertyID)
        {
            int rowsAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", PropertyID);

                conn.Open();
                rowsAffected = cmd.ExecuteNonQuery();
            }
            return rowsAffected > 0;
        }
        #endregion


        #region UpdatePropertyStatusToSold
        public bool UpdatePropertyStatusToSold(int propertyID, int userID)
        {
            int rowsAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdatePropertyStatusToSold", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", propertyID);
                cmd.Parameters.AddWithValue("@UserID", userID);

                // Add output parameter
                SqlParameter outputParam = new SqlParameter("@RowsAffected", SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                };
                cmd.Parameters.Add(outputParam);

                conn.Open();
                cmd.ExecuteNonQuery();
                rowsAffected = (int)outputParam.Value;
            }
            return rowsAffected > 0;
        }
        #endregion



        #region SelectPropertiesByBuyer
        public List<PropertyModel> SelectPropertiesByBuyer(int userID)
        {
            var properties = new List<PropertyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
               
                SqlCommand cmd = new SqlCommand("GetBuyerProperties", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    properties.Add(new PropertyModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        Title = reader["Title"].ToString(),
                        Status = reader["Status"].ToString(),
                        Type = reader["Type"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        // Reading the new fields. Ensure your stored procedure returns these columns.
                        CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : null,
                        BuyerName = reader["BuyerName"] != DBNull.Value ? reader["BuyerName"].ToString() : null
                    });
                }
            }
            return properties;
        }
        #endregion

        #region SearchProperties
        public IEnumerable<PropertyModel> SearchProperties(string location = null, decimal? minPrice = null, decimal? maxPrice = null, string type = null)
        {
            var properties = new List<PropertyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SearchProperties", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                
                cmd.Parameters.AddWithValue("@Location", string.IsNullOrEmpty(location) ? DBNull.Value : location);
                cmd.Parameters.AddWithValue("@MinPrice", minPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaxPrice", maxPrice ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Type", string.IsNullOrEmpty(type) ? DBNull.Value : type);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    properties.Add(new PropertyModel
                    {
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Description = reader["Description"].ToString(),
                        Location = reader["Location"].ToString(),
                        Title = reader["Title"].ToString(),
                        Status = reader["Status"].ToString(),
                        Type = reader["Type"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        UserID = Convert.ToInt32(reader["UserID"]),
                        CreatedBy = reader["UserName"] != DBNull.Value ? reader["UserName"].ToString() : null
                    });
                }
            }
            return properties;
        }
        #endregion

    }
}
