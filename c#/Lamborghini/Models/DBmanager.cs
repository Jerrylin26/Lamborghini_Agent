using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Lamborghini.Models
{
    public class DBmanager
    {
        private readonly string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Database=account;User ID=Jerry;Password=lccJerry1;Trusted_Connection=True";

        // 讀取
        public List<account> getAccount()
        {
            List<account> accounts = new List<account>();

            SqlConnection sqlConnection = new SqlConnection(connStr);
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM member");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    account account = new account
                    {
                        ID = reader.GetInt32(reader.GetOrdinal("id")),
                        userName = reader.GetString(reader.GetOrdinal("userName")),
                        password = reader.GetString(reader.GetOrdinal("password")),
                        age = reader.GetDouble(reader.GetOrdinal("age")),
                    };
                    accounts.Add(account);
                }
            }
            else
            {
                Console.WriteLine("資料庫為空！");
            }
            sqlConnection.Close();
            return accounts;
        }

        // 寫入
        public void newAccount(account user)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand(@"INSERT INTO member(username,password,age) VALUES(@username,@password,@age)");
            sqlcommand.Connection = sqlconnection;

            sqlcommand.Parameters.Add(new SqlParameter("@username", user.userName));
            sqlcommand.Parameters.Add(new SqlParameter("@password", user.password));
            sqlcommand.Parameters.Add(new SqlParameter("@age", user.age));

            sqlconnection.Open();
            sqlcommand.ExecuteNonQuery();
            sqlconnection.Close();
        }

        // 修改



        // 刪除
    }
}
