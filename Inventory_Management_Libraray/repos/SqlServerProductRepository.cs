using Inventory_Management_Libraray.interfaces;
using Inventory_Management_Library;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace Inventory_Management_Libraray.repos
{
    public class SqlServerProductRepository : IProductRepository
    {
        private SqlConnection _connection;

        public SqlServerProductRepository()
        {
            try
            {
                var builder = new SqlConnectionStringBuilder();
                builder.DataSource = "LAPTOP-TCMGSSUR";
                builder.InitialCatalog = "Simple_Inventory_Management_System";
                builder.IntegratedSecurity = true;
                builder.Encrypt = false;

                _connection = new SqlConnection(builder.ConnectionString);
            }
            catch (SqlException e)
            {
                Console.WriteLine(e.ToString());
            }

        }

        ~SqlServerProductRepository()
        {
            _connection.Close();
            _connection.Dispose();
        }
        public bool AddProduct(Product product)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO Products(name, price, quantity) VALUES (@p_name, @p_price, @p_quantity)", _connection);
                cmd.Parameters.AddWithValue("@p_name", product.Name);
                cmd.Parameters.AddWithValue("@p_price", product.Price);
                cmd.Parameters.AddWithValue("@p_quantity", product.Quantity);

                _connection.Open();
                int numOfRows = cmd.ExecuteNonQuery();
                _connection.Close();

                if (numOfRows > 0)
                {
                    return true;
                }

            }
            catch (SqlException ex)
            {
                foreach (SqlError error in ex.Errors)
                {
                    Console.WriteLine($"SQL Server Error: {error.Message}");
                }
            }

            return false;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();

            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products;", _connection))
                {
                    _connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fetchedProduct = new Product() { Name = reader.GetString(0), Price = (float)reader.GetDouble(1) };
                            fetchedProduct.IncreaseQuantity(reader.GetInt32(2));
                            products.Add(fetchedProduct);
                        }
                    }
                }

            }
            catch (SqlException ex)
            {
                foreach (SqlError error in ex.Errors)
                {
                    Console.WriteLine($"SQL Server Error: {error.Message}");
                }
            }
            finally { _connection.Close(); }

            return products;
        }

        public Product GetProduct(string productName)
        {
            Product fetchedProduct = null;
            try
            {
                using (var cmd = new SqlCommand("SELECT * FROM Products WHERE name = @p_name", _connection))
                {
                    cmd.Parameters.AddWithValue("@p_name", productName);
                    _connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        if (reader.Read())
                        {
                            fetchedProduct = new Product();
                            fetchedProduct.Name = reader.GetString(0);
                            fetchedProduct.Price = (float)reader.GetDouble(1);
                            fetchedProduct.IncreaseQuantity(reader.GetInt32(2));
                        }
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            finally { _connection.Close(); }

            return fetchedProduct;
        }

        public bool RemoveProduct(string productName)
        {
            try
            {
                using (var cmd = new SqlCommand("DELETE FROM Products WHERE name = @p_name", _connection))
                {

                    cmd.Parameters.AddWithValue("@p_name", productName);
                    _connection.Open();
                    int rowNum = cmd.ExecuteNonQuery();

                    if (rowNum > 0)
                    {
                        return true;
                    }
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _connection.Close();
            }
            return false;
        }

        public bool UpdateProduct(string productName, Product newProduct)
        {

            if (productName == newProduct.Name)
            {
                return UpdateProductWithoutPrimary(productName, newProduct);
            }

            return UpdateProductWithPrimary(productName, newProduct);

        }
        private bool UpdateProductWithoutPrimary(string productName, Product newProduct)
        {
            var success = false;
            try
            {
                using (var cmd = new SqlCommand())
                {
                    cmd.Connection = _connection;
                    _connection.Open();
                    success = UpdateProductCommand(productName, newProduct, cmd);

                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _connection.Close();
            }
            return success;
        }
        private bool UpdateProductWithPrimary(string productName, Product newProduct)
        {
            var updateState = false;
            try
            {
                updateState = SafeProductUpdate(productName, newProduct);
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                _connection.Close();
            }
            return updateState;
        }
        private bool SafeProductUpdate(string productName, Product newProduct)
        {
            bool updateState = false;
            _connection.Open();
            SqlTransaction transaction = _connection.BeginTransaction();
            try
            {
                
                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Transaction = transaction;
                    sqlCommand.Connection = _connection;

                    if (IsProductNameReserved(newProduct.Name, sqlCommand))
                        return false;

                    DeleteProductCommand(productName, sqlCommand); // delete old product
                    InsertProductCommand(newProduct, sqlCommand); // inserts new one
                }
                transaction.Commit();
                updateState = true;
            }
            catch (SqlException ex)
            {
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
            }
            return updateState;
        }

        private bool IsProductNameReserved(string name, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = "SELECT COUNT('*') FROM Products WHERE name = @p_name";
            sqlCommand.Parameters.AddWithValue("@p_name", name);

            int count = (int)sqlCommand.ExecuteScalar();
            sqlCommand.Parameters.Clear();
            if (count != 0)
            {
                return true;
            }
            return false;
        }
        private bool DeleteProductCommand(string productName, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = "DELETE FROM Products WHERE name = @p_name";
            sqlCommand.Parameters.AddWithValue("@p_name", productName);

            int rowNum = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();

            return (rowNum > 0);
        }

        private bool InsertProductCommand(Product newProduct, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = "INSERT INTO Products(name, price, quantity) VALUES (@p_name, @p_price, @p_quantity)";
            sqlCommand.Parameters.AddWithValue("@p_name", newProduct.Name);
            sqlCommand.Parameters.AddWithValue("@p_price", newProduct.Price);
            sqlCommand.Parameters.AddWithValue("@p_quantity", newProduct.Quantity);

            int rowNum = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();

            return (rowNum > 0);
        }

        private bool UpdateProductCommand(string productName, Product newProduct, SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE Products SET Price = @p_price, Quantity = @p_quan WHERE name = @p_name";

            cmd.Parameters.AddWithValue("@p_name", productName);
            cmd.Parameters.AddWithValue("@p_price", newProduct.Price);
            cmd.Parameters.AddWithValue("@p_quan", newProduct.Quantity);

            int rowNum = cmd.ExecuteNonQuery();
            return (rowNum > 0);
        }
    }
}
