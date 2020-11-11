using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TenmoServer.Models;
using TenmoServer.Security;
using TenmoServer.Security.Models;

namespace TenmoServer.DAO
{
    public class UserSqlDAO : IUserDAO
    {
        private readonly string connectionString;
        const decimal startingBalance = 1000;


        const string sql_getUser = "SELECT user_id, username, password_hash, salt FROM users WHERE username = @username";
        const string sql_getUsers = "SELECT user_id, username, password_hash, salt FROM users";
        const string sql_addUser = "INSERT INTO users (username, password_hash, salt) VALUES (@username, @password_hash, @salt)";
        const string sql_getBalance = "SELECT balance FROM users u JOIN accounts a ON u.user_id = a.user_id WHERE a.user_id = @user_id";
        const string sql_transferMoneyFrom = "UPDATE accounts SET balance = (balance - @amountToTransfer) WHERE account_id = @user_id";
        const string sql_transferMoneyTo = "UPDATE accounts SET balance = (balance + @amountToTransfer) WHERE account_id = @accountToTransfer_id";
        const string sql_transferStatus = "INSERT INTO transfers VALUES (2, 2, @user_id, @accountToTransfer_id, @amountToTransfer)";
        const string sql_transferViewer = "SELECT * FROM transfers WHERE account_from = @user_id OR account_to = @user_id";
        const string sql_transferSearch = "SELECT * FROM transfers WHERE transfer_id = @transfer_id";


        public UserSqlDAO(string dbConnectionString)
        {
            connectionString = dbConnectionString;
        }

        public User GetUser(string username)
        {
            User returnUser = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_getUser, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows && reader.Read())
                    {
                        returnUser = GetUserFromReader(reader);
                    }
                }
            }
            catch (SqlException ex)
            {
                throw;
            }

            return returnUser;
        }

        public List<User> GetUsers()
        {
            List<User> returnUsers = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_getUsers, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            User u = GetUserFromReader(reader);
                            returnUsers.Add(u);
                        }

                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return returnUsers;
        }

        public User AddUser(string username, string password)
        {
            IPasswordHasher passwordHasher = new PasswordHasher();
            PasswordHash hash = passwordHasher.ComputeHash(password);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_addUser, conn);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password_hash", hash.Password);
                    cmd.Parameters.AddWithValue("@salt", hash.Salt);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("SELECT @@IDENTITY", conn);
                    int userId = Convert.ToInt32(cmd.ExecuteScalar());

                    cmd = new SqlCommand("INSERT INTO accounts (user_id, balance) VALUES (@userid, @startBalance)", conn);
                    cmd.Parameters.AddWithValue("@userid", userId);
                    cmd.Parameters.AddWithValue("@startBalance", startingBalance);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw;
            }

            return GetUser(username);
        }

        private User GetUserFromReader(SqlDataReader reader)
        {
            User u = new User()
            {
                UserId = Convert.ToInt32(reader["user_id"]),
                Username = Convert.ToString(reader["username"]),
                PasswordHash = Convert.ToString(reader["password_hash"]),
                Salt = Convert.ToString(reader["salt"])
            };

            return u;
        }

        public double GetBalance(int id)
        {
            double currentBalance = 0;


            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_getBalance, conn);
                    cmd.Parameters.AddWithValue("@user_id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        currentBalance = Convert.ToDouble(reader["balance"]);
                    }


                }
            }


            catch (Exception ex)
            {

            }

            return currentBalance;
        }


        public bool TransferMoneyFrom(int id, double amountToDeduct)
        {
            bool done = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_transferMoneyFrom, conn);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    cmd.Parameters.AddWithValue("@amountToTransfer", amountToDeduct);

                    cmd.ExecuteNonQuery();
                    done = true;
                }

            }


            catch (Exception ex)
            {
                return done;
            }


            return done;
        }


        public bool TransferMoneyTo(int id, int idToSendTo, double amountToDeduct)
        {
            bool done = false;
            try
            {

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_transferMoneyTo, conn);
                    cmd.Parameters.AddWithValue("@accountToTransfer_id", idToSendTo);
                    cmd.Parameters.AddWithValue("@amountToTransfer", amountToDeduct);

                    cmd.ExecuteNonQuery();
                    done = true;

                    conn.Close();

                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_transferStatus, conn);
                    cmd.Parameters.AddWithValue("@user_id", id);
                    cmd.Parameters.AddWithValue("@accountToTransfer_id", idToSendTo);
                    cmd.Parameters.AddWithValue("@amountToTransfer", amountToDeduct);

                    cmd.ExecuteNonQuery();
                }


            }

            catch (Exception ex)
            {
                return done;
            }

            return done;
        }

        public List<TransferMoney> ViewTransfer(int id)
        {

            
            List<TransferMoney> transfers = new List<TransferMoney>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_transferViewer, conn);
                    cmd.Parameters.AddWithValue("@user_id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int transfer_id = Convert.ToInt32(reader["transfer_id"]);
                        int transfer_type_id = Convert.ToInt32(reader["transfer_type_id"]);
                        int transfer_status_id = Convert.ToInt32(reader["transfer_status_id"]);
                        int userId = Convert.ToInt32(reader["account_from"]);
                        int transferToId = Convert.ToInt32(reader["account_to"]);
                        double amount = Convert.ToInt32(reader["amount"]);

                        TransferMoney transfer = new TransferMoney(transfer_id, transfer_type_id, transfer_status_id, userId, transferToId, amount);
                        transfers.Add(transfer);
                    }
                }
            }

            catch (Exception ex)
            {

            }


            return transfers;
        }

        public List<TransferMoney> TransferSearch(int transfer_Id)
        {
            
            List<TransferMoney> transfers = new List<TransferMoney>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql_transferSearch, conn);
                    cmd.Parameters.AddWithValue("@transfer_id", transfer_Id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        int transfer_id = Convert.ToInt32(reader["transfer_id"]);
                        int transfer_type_id = Convert.ToInt32(reader["transfer_type_id"]);
                        int transfer_status_id = Convert.ToInt32(reader["transfer_status_id"]);
                        int userId = Convert.ToInt32(reader["account_from"]);
                        int transferToId = Convert.ToInt32(reader["account_to"]);
                        double amount = Convert.ToInt32(reader["amount"]);

                        TransferMoney transfer = new TransferMoney(transfer_id, transfer_type_id, transfer_status_id, userId, transferToId, amount);
                        transfers.Add(transfer);
                    }
                }
            }

            catch (Exception ex)
            {

            }


            return transfers;
        }
    }
}
