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
        private readonly string connStr = "Data Source=(localdb)\\MSSQLLocalDB;Database=lamborghini;User ID=Jerry;Password=lccJerry1;Trusted_Connection=True";


        // 讀取Member資料表
        public List<Member> getAccount()
        {
            List<Member> accounts = new List<Member>();

            SqlConnection sqlConnection = new SqlConnection(connStr);
            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Member");
            sqlCommand.Connection = sqlConnection;

            SqlDataReader reader = sqlCommand.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Member account = new Member
                    {
                        MID = reader.GetInt32(reader.GetOrdinal("MID")),
                        Name = reader.GetString(reader.GetOrdinal("Name")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        Phone = reader.GetString(reader.GetOrdinal("Phone")),
                        Address = reader.GetString(reader.GetOrdinal("Address")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Account = reader.GetString(reader.GetOrdinal("Account")),
                        
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

        // 寫入Member資料表
        public void newAccount(Member user)
        {
            SqlConnection sqlconnection = new SqlConnection(connStr);
            SqlCommand sqlcommand = new SqlCommand(@"INSERT INTO Member(Password,Account,Name,Address,Phone,Email) VALUES(@password,@account,@name,@address,@phone,@email)");
            sqlcommand.Connection = sqlconnection;

            sqlcommand.Parameters.Add(new SqlParameter("@name", user.Name));
            sqlcommand.Parameters.Add(new SqlParameter("@password", user.Password));
            sqlcommand.Parameters.Add(new SqlParameter("@account", user.Account));
            sqlcommand.Parameters.Add(new SqlParameter("@email", user.Email));
            sqlcommand.Parameters.Add(new SqlParameter("@address", user.Address));
            sqlcommand.Parameters.Add(new SqlParameter("@phone", user.Phone));


            sqlconnection.Open();
            sqlcommand.ExecuteNonQuery();
            sqlconnection.Close();
        }

        // 修改Member資料表



        // 刪除Member資料表


        /*---------------------------------------------------------------------------------------*/
        // 以下為初始寫入資料庫

        // 初始寫入 Product資料表
        public int newProduct(Product user)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO Product(Price, CarSeries, CarModel, IsDisplay)
                    VALUES(@price, @carseries, @carmodel, @isdisplay);
                    SELECT SCOPE_IDENTITY();", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@price", user.Price);
                sqlcommand.Parameters.AddWithValue("@carseries", user.CarSeries);
                sqlcommand.Parameters.AddWithValue("@carmodel", user.CarModel);
                sqlcommand.Parameters.AddWithValue("@isdisplay", user.IsDisplay);

                // 取得剛插入的 ProductID
                return Convert.ToInt32(sqlcommand.ExecuteScalar());
            }
        }

        // 初始寫入 ProductImage資料表
        public void newProductImage(int productId, string img)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO ProductImage(Img, ProductID)
                    VALUES(@img, @productId);", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@img", img);
                sqlcommand.Parameters.AddWithValue("@productId", productId);

                sqlcommand.ExecuteNonQuery();
            }
        }


        // 初始寫入 AccessoryProduct資料表
        public int newAccessoryProduct(AccessoryProduct user)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO AccessoryProduct(Price, Collection, Item, IsDisplay, Description)
                    VALUES(@price, @collection, @item, @isdisplay, @description);
                    SELECT SCOPE_IDENTITY();", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@price", user.Price);
                sqlcommand.Parameters.AddWithValue("@collection", user.Collection);
                sqlcommand.Parameters.AddWithValue("@item", user.Item);
                sqlcommand.Parameters.AddWithValue("@isdisplay", user.IsDisplay);
                sqlcommand.Parameters.AddWithValue("@description", user.Description);

                // 取得剛插入的 AccessoryProductID
                return Convert.ToInt32(sqlcommand.ExecuteScalar());
            }
        }

        // 初始寫入 AccessoryProductImage資料表
        public void newAccessoryProductImage(int productId, string img)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO AccessoryProductImage(Img, ProductID)
                    VALUES(@img, @productId);", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@img", img);
                sqlcommand.Parameters.AddWithValue("@productId", productId);

                sqlcommand.ExecuteNonQuery();
            }
        }


    }
}
