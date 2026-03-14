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

        #region Admin Properties
        public IEnumerable<PropertyModel> GetUnapprovedProperties()
        {
            var properties = new List<PropertyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetUnapprovedProperties", conn)
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
                        UserID = Convert.ToInt32(reader["UserID"]),
                        CreatedBy = reader["CreatedBy"] != DBNull.Value ? reader["CreatedBy"].ToString() : string.Empty,
                        SellerPhone = reader["SellerPhone"] != DBNull.Value ? reader["SellerPhone"].ToString() : string.Empty,
                        RejectionReason = reader["RejectionReason"] != DBNull.Value ? reader["RejectionReason"].ToString() : string.Empty
                    });
                }
            }
            return properties;
        }

        public bool ApproveProperty(int propertyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("ApproveProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", propertyID);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool RejectProperty(int propertyID, string rejectionReason)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("RejectProperty", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", propertyID);
                cmd.Parameters.AddWithValue("@RejectionReason", string.IsNullOrEmpty(rejectionReason) ? DBNull.Value : (object)rejectionReason);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
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
                        UserID = Convert.ToInt32(reader["UserID"]),
                        CreatedBy = reader["CreatedBy"].ToString(),
                        SellerPhone = reader["SellerPhone"] != DBNull.Value ? reader["SellerPhone"].ToString() : string.Empty,
                        BuyerID = reader["BuyerID"] != DBNull.Value ? Convert.ToInt32(reader["BuyerID"]) : (int?)null,
                        RejectionReason = reader["RejectionReason"] != DBNull.Value ? reader["RejectionReason"].ToString() : string.Empty
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
                // When modifying from frontend after a rejection, Status goes back to "Pending"
                if (property.Status == "Pending")
                {
                    cmd.Parameters.AddWithValue("@Status", "Pending");
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Status", property.Status);
                }
                
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
                conn.Open();
                
                // First try the stored procedure
                try
                {
                    SqlCommand cmd = new SqlCommand("DeleteProperty", conn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    cmd.Parameters.AddWithValue("@PropertyID", PropertyID);
                    rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0) return true;
                }
                catch
                {
                    // SP may not handle cascading deletes, fall through to manual cleanup
                }

                // Manual cascading delete: clean up all child tables first
                try
                {
                    string[] cleanupQueries = new[]
                    {
                        "DELETE FROM Feedback WHERE PropertyID = @PropertyID",
                        "DELETE FROM [Transaction] WHERE PropertyID = @PropertyID",
                        "DELETE FROM Favorites WHERE PropertyID = @PropertyID",
                        "DELETE FROM UserFavorites WHERE PropertyID = @PropertyID",
                        "DELETE FROM PropertyImages WHERE PropertyID = @PropertyID",
                        "DELETE FROM Property WHERE PropertyID = @PropertyID"
                    };

                    foreach (var query in cleanupQueries)
                    {
                        try
                        {
                            SqlCommand cmd = new SqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@PropertyID", PropertyID);
                            cmd.ExecuteNonQuery();
                        }
                        catch { /* Table may not exist, skip */ }
                    }
                    return true;
                }
                catch
                {
                    return false;
                }
            }
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

        #region CancelLease
        public bool CancelLease(int propertyID, int userID)
        {
            int rowsAffected = 0;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                try
                {
                    SqlCommand cmd = new SqlCommand("CancelLease", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@PropertyID", propertyID);
                    cmd.Parameters.AddWithValue("@UserID", userID);
                    rowsAffected = cmd.ExecuteNonQuery();
                }
                catch
                {
                    try
                    {
                        SqlCommand cmdRaw = new SqlCommand("UPDATE Property SET Status = 'Pending', BuyerID = NULL WHERE PropertyID = @PropertyID", conn);
                        cmdRaw.Parameters.AddWithValue("@PropertyID", propertyID);
                        rowsAffected = cmdRaw.ExecuteNonQuery();
                    }
                    catch { }
                }
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
                        BuyerName = reader["BuyerName"] != DBNull.Value ? reader["BuyerName"].ToString() : null,
                        RejectionReason = reader["RejectionReason"] != DBNull.Value ? reader["RejectionReason"].ToString() : null
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

        #region Favorites
        public string ToggleFavorite(int userID, int propertyID)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("ToggleFavorite", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@UserID", userID);
                cmd.Parameters.AddWithValue("@PropertyID", propertyID);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "Error";
            }
        }

        public IEnumerable<PropertyModel> GetUserFavorites(int userID)
        {
            var properties = new List<PropertyModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("GetUserFavorites", conn)
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
                        Title = reader["Title"].ToString(),
                        Location = reader["Location"].ToString(),
                        Description = reader["Description"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Type = reader["Type"].ToString(),
                        Status = reader["Status"].ToString(),
                        ImageUrl = reader["ImageUrl"].ToString()
                    });
                }
            }
            return properties;
        }
        #endregion

    }
}
