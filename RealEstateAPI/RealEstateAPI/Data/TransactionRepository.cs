using Microsoft.Data.SqlClient;
using RealEstateAPI.Model;
using System.Data;

namespace RealEstateAPI.Data
{
    public class TransactionRepository
    {
        private readonly string _connectionString;
        public TransactionRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region SelectAllTransactions

        public IEnumerable<TransactionModel> SelectAllTransaction()
        {
            var transaction = new List<TransactionModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectAllTransactions", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    transaction.Add(
                        new TransactionModel
                        {

                            TransactionID = Convert.ToInt32(reader["TransactionID"]),
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            Amount = Convert.ToDecimal(reader["Price"]),
                            BuyerID = Convert.ToInt32(reader["BuyerID"]),
                            SellerID = Convert.ToInt32(reader["SellerID"]),
                            TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                            Type=reader["Type"].ToString()                            
                        });

                }
            }
            return transaction;
        }
        #endregion

        #region SelectTransactionsByID
        public IEnumerable<TransactionModel> SelectTransactionByID(int TransactionID)
        {
            var transaction = new List<TransactionModel>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("SelectTransactionByID", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                cmd.Parameters.AddWithValue("@TransactionID", TransactionID);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    transaction.Add(
                        new TransactionModel
                        {
                            TransactionID = Convert.ToInt32(reader["TransactionID"]),
                            PropertyID = Convert.ToInt32(reader["PropertyID"]),
                            Amount = Convert.ToDecimal(reader["Price"]),
                            BuyerID = Convert.ToInt32(reader["BuyerID"]),
                            SellerID = Convert.ToInt32(reader["SellerID"]),
                            TransactionDate = Convert.ToDateTime(reader["TransactionDate"]),
                            Type = reader["Type"].ToString()
                        });
                }
            }
            return transaction;
        }
        #endregion

        #region InsertTransaction
        public bool InsertTransaction(TransactionModel transaction)
        {
            int rowAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("InsertTransaction", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();

                cmd.Parameters.AddWithValue("@Type", transaction.Type);
                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                cmd.Parameters.AddWithValue("@SellerID", transaction.SellerID);
                cmd.Parameters.AddWithValue("@BuyerID", transaction.BuyerID);
                cmd.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                cmd.Parameters.AddWithValue("@PropertyID", transaction.PropertyID);



                rowAffected = cmd.ExecuteNonQuery();

            }
            return rowAffected > 0;
        }

        #endregion

        #region UpdateTransaction
        public bool UpdateTransaction(TransactionModel transaction)
        {
            int rowAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("UpdateTransaction", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                cmd.Parameters.AddWithValue("@TransactionID", transaction.TransactionID);
                cmd.Parameters.AddWithValue("@Type", transaction.Type);
                cmd.Parameters.AddWithValue("@Amount", transaction.Amount);
                cmd.Parameters.AddWithValue("@SellerID", transaction.SellerID);
                cmd.Parameters.AddWithValue("@BuyerID", transaction.BuyerID);
                cmd.Parameters.AddWithValue("@TransactionDate", transaction.TransactionDate);
                cmd.Parameters.AddWithValue("@PropertyID", transaction.PropertyID);

                rowAffected = cmd.ExecuteNonQuery();

            }
            return rowAffected > 0;
        }

        #endregion

        #region DeleteTransaction
        public bool DeleteTransaction(int TransactionID)
        {
            int rowAffected;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                SqlCommand cmd = new SqlCommand("DeleteTransaction", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                conn.Open();
                cmd.Parameters.AddWithValue("@TransactionID", TransactionID);
                rowAffected = cmd.ExecuteNonQuery();

            }
            return rowAffected > 0;
        }
        #endregion
    }
}
