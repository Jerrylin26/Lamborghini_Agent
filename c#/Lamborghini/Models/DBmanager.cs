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


        /*---------------------------------------------------------------------------------------*/
        // 以下為 Member 資料表的 CRUD
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

        /*---------------------------------------------------------------------------------------*/
        // 以下為 Product 資料表的 CRUD

        // 讀取Product資料表 根據Series
        public List<Product> getProduct(string Series)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();
                SqlCommand sqlcommand = new SqlCommand();


                sqlcommand = new SqlCommand(@"
                SELECT 
                    p.PID AS ProductID,
                    p.Price,
                    p.CarSeriesID,
                    p.CarModel,
                    p.IsDisplay,
                    c.SeriesName,
                    pp.ImgFileName
                    
                FROM Product p
                JOIN CarSeriesID c on p.CarSeriesID = c.CarSeriesID
                LEFT JOIN ProductImage pp on pp.ProductID = p.PID
                WHERE c.SeriesName = @carseries
                ", sqlconnection);


                sqlcommand.Parameters.AddWithValue("@carseries", Series);
                


                SqlDataReader reader = sqlcommand.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        PID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        Price = reader.GetDouble(reader.GetOrdinal("Price")),
                        CarSeriesID = reader.GetInt32(reader.GetOrdinal("CarSeriesID")),
                        CarModel = reader.GetString(reader.GetOrdinal("CarModel")),
                        IsDisplay = reader.GetBoolean(reader.GetOrdinal("IsDisplay")),
                        CarSeries = reader.GetString(reader.GetOrdinal("SeriesName")),
                        Img = reader.GetString(reader.GetOrdinal("ImgFileName"))
                    };
                    products.Add(product);
                }

            }
            return products;
        }


        // 讀取Product資料表 單一車款詳細資料
        public List<Product> getDetail(string Series, string Model)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();
                SqlCommand sqlcommand = new SqlCommand();


                sqlcommand = new SqlCommand(@"
                SELECT 
                    p.PID AS ProductID,
                    p.Price,
                    p.CarSeriesID,
                    p.CarModel,
                    p.IsDisplay,
                    c.SeriesName,
                    pp.ImgFileName
                    
                FROM Product p
                JOIN CarSeriesID c on p.CarSeriesID = c.CarSeriesID
                LEFT JOIN ProductImage pp on pp.ProductID = p.PID
                WHERE c.SeriesName = @carseries
                AND p.CarModel = @carmodel
                ", sqlconnection);


                sqlcommand.Parameters.AddWithValue("@carseries", Series);
                sqlcommand.Parameters.AddWithValue("@carmodel", Model);



                SqlDataReader reader = sqlcommand.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        PID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        Price = reader.GetDouble(reader.GetOrdinal("Price")),
                        CarSeriesID = reader.GetInt32(reader.GetOrdinal("CarSeriesID")),
                        CarModel = reader.GetString(reader.GetOrdinal("CarModel")),
                        IsDisplay = reader.GetBoolean(reader.GetOrdinal("IsDisplay")),
                        CarSeries = reader.GetString(reader.GetOrdinal("SeriesName")),
                        Img = reader.GetString(reader.GetOrdinal("ImgFileName"))
                    };
                    products.Add(product);
                }

            }
            return products;
        }

        // 讀取Product資料表 單一車款詳細資料 (int PID)
        public List<Product> getDetail(int PID)
        {
            List<Product> products = new List<Product>();
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();
                SqlCommand sqlcommand = new SqlCommand();


                sqlcommand = new SqlCommand(@"
                SELECT 
                    p.PID AS ProductID,
                    p.Price,
                    p.CarSeriesID,
                    p.CarModel,
                    p.IsDisplay,
                    c.SeriesName,
                    pp.ImgFileName
                    
                FROM Product p
                JOIN CarSeriesID c on p.CarSeriesID = c.CarSeriesID
                LEFT JOIN ProductImage pp on pp.ProductID = p.PID
                WHERE p.PID = @pid
                ", sqlconnection);


                sqlcommand.Parameters.AddWithValue("@pid", PID);


                SqlDataReader reader = sqlcommand.ExecuteReader();
                while (reader.Read())
                {
                    Product product = new Product
                    {
                        PID = reader.GetInt32(reader.GetOrdinal("ProductID")),
                        Price = reader.GetDouble(reader.GetOrdinal("Price")),
                        CarSeriesID = reader.GetInt32(reader.GetOrdinal("CarSeriesID")),
                        CarModel = reader.GetString(reader.GetOrdinal("CarModel")),
                        IsDisplay = reader.GetBoolean(reader.GetOrdinal("IsDisplay")),
                        CarSeries = reader.GetString(reader.GetOrdinal("SeriesName")),
                        Img = reader.GetString(reader.GetOrdinal("ImgFileName"))
                    };
                    products.Add(product);
                }

            }
            return products;
        }








        /*---------------------------------------------------------------------------------------*/
        // 以下為初始寫入資料庫

        // 初始寫入 Product資料表
        public int newProduct(Product user)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO Product(Price, CarSeriesID, CarModel, IsDisplay)
                    VALUES(@price, @carseriesId, @carmodel, @isdisplay);
                    SELECT SCOPE_IDENTITY();", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@price", user.Price);
                sqlcommand.Parameters.AddWithValue("@carseriesId", user.CarSeriesID);
                sqlcommand.Parameters.AddWithValue("@carmodel", user.CarModel);
                sqlcommand.Parameters.AddWithValue("@isdisplay", user.IsDisplay);

                // 取得剛插入的 ProductID
                return Convert.ToInt32(sqlcommand.ExecuteScalar());
            }
        }

        // 初始寫入 ProductImage資料表
        public void newProductImage(int productId, Product user)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO ProductImage(ImgFileName, ProductID, CarSeriesID)
                    VALUES(@img, @productId, @carseriesId);", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@img", user.Img);
                sqlcommand.Parameters.AddWithValue("@productId", productId);
                sqlcommand.Parameters.AddWithValue("@carseriesId", user.CarSeriesID);

                sqlcommand.ExecuteNonQuery();
            }
        }

        // 初始寫入 CarSeriesID 資料表 (隸屬於 Product資料表)
        public void newCarSeriesID(Product user)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO CarSeriesID(CarSeriesID, SeriesName)
                    VALUES( @carseriesId, @SeriesName);
                    SELECT SCOPE_IDENTITY();", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@carseriesId", user.CarSeriesID);
                sqlcommand.Parameters.AddWithValue("@SeriesName", user.CarSeries);


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
                    INSERT INTO AccessoryProduct(Price, CollectionID, Item, IsDisplay, Description)
                    VALUES(@price, @collectionID, @item, @isdisplay, @description);
                    SELECT SCOPE_IDENTITY();", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@price", user.Price);
                sqlcommand.Parameters.AddWithValue("@collectionID", user.CollectionID);
                sqlcommand.Parameters.AddWithValue("@item", user.Item);
                sqlcommand.Parameters.AddWithValue("@isdisplay", user.IsDisplay);
                sqlcommand.Parameters.AddWithValue("@description", user.Description);

                // 取得剛插入的 AccessoryProductID
                return Convert.ToInt32(sqlcommand.ExecuteScalar());
            }
        }

        // 初始寫入 AccessoryProductImage資料表
        public void newAccessoryProductImage(int productId, AccessoryProduct user)
        {
            using (SqlConnection sqlconnection = new SqlConnection(connStr))
            {
                sqlconnection.Open();

                SqlCommand sqlcommand = new SqlCommand(@"
                    INSERT INTO AccessoryProductImage(ImgSrc, ProductID, CollectionID)
                    VALUES(@img, @productId, @collectionID);", sqlconnection);

                sqlcommand.Parameters.AddWithValue("@img", user.Img);
                sqlcommand.Parameters.AddWithValue("@productId", productId);
                sqlcommand.Parameters.AddWithValue("@collectionID", user.CollectionID);

                sqlcommand.ExecuteNonQuery();
            }
        }


    }
}
