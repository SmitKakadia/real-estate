using Microsoft.Data.SqlClient;
using RealEstateAPI.Model;
using System.Data;

namespace RealEstateAPI.Data
{
    public class FeedbackRepository
    {
        private readonly string _connectionString;
        public FeedbackRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAllFeedbacks

        public IEnumerable<FeedbackModel> SelectAllFeedback()
        {
            var feedback = new List<FeedbackModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectAllFeedbacks", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    feedback.Add(
                        new FeedbackModel
                        {

                            FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            Description = reader["Description"].ToString(),
                            UserID = Convert.ToInt32(reader["UserID"])
                            
                        });

                }
            }
            return feedback;
        }
        #endregion

        #region SelectFeedbacksByID
        public IEnumerable<FeedbackModel> SelectFeedbackByID(int FeedbackID)
        {
            var feedback = new List<FeedbackModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectFeedbackByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                cmd.Parameters.AddWithValue("@FeedbackID", FeedbackID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    feedback.Add(
                        new FeedbackModel
                        {
                            FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            Description = reader["Description"].ToString(),
                            UserID = Convert.ToInt32(reader["UserID"])
                        });
                }
            }
            return feedback;
        }
        #endregion

        #region InsertFeedback
        public bool InsertFeedback(FeedbackModel feedback)
        {
            int rowAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertFeedback", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();

                cmd.Parameters.AddWithValue("@Description", feedback.Description);
                cmd.Parameters.AddWithValue("@UserID", feedback.UserID);
                cmd.Parameters.AddWithValue("@PropertyID", feedback.PropertyID);



                rowAffected = cmd.ExecuteNonQuery();

            }
            return rowAffected > 0;
        }

        #endregion

        #region UpdateFeedback
        public bool UpdateFeedback(FeedbackModel feedback)
        {
            int rowAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateFeedback", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                cmd.Parameters.AddWithValue("@FeedbackID", feedback.FeedbackID);
                cmd.Parameters.AddWithValue("@Description", feedback.Description);
                cmd.Parameters.AddWithValue("@UserID", feedback.UserID);
                cmd.Parameters.AddWithValue("@PropertyID", feedback.PropertyID);

                rowAffected = cmd.ExecuteNonQuery();

            }
            return rowAffected > 0;
        }

        #endregion

        #region DeleteFeedback
        public bool DeleteFeedback(int FeedbackID)
        {
            int rowAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteFeedback", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                cmd.Parameters.AddWithValue("@FeedbackID", FeedbackID);
                rowAffected = cmd.ExecuteNonQuery();

            }
            return rowAffected > 0;
        }
        #endregion

        #region SelectFeedbacksByPropertyID
        public IEnumerable<FeedbackModel> SelectFeedbackByPropertyID(int propertyID)
        {
            var feedback = new List<FeedbackModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
               
                SqlCommand cmd = new SqlCommand("GetFeedbackByPropertyID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@PropertyID", propertyID);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    feedback.Add(new FeedbackModel
                    {
                        FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                        PropertyID = Convert.ToInt32(reader["PropertyID"]),
                        Description = reader["Description"].ToString(),
                        UserID = Convert.ToInt32(reader["UserID"])
                    });
                }
            }
            return feedback;
        }
        #endregion
    }
}
