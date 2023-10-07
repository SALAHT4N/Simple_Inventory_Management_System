using Inventory_Management_Libraray.interfaces;
using Inventory_Management_Library;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace Inventory_Management_Libraray.repos
{
    public class SqlServerProductRepository : IProductRepository
    {
        private SqlConnection _connection;

        public SqlServerProductRepository()
        {
            // make a generic repo for product that takes in a database connection as DbConnection
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
            _connection.Dispose();
        }
        public bool AddProduct(Product product)
        {
            // make scoped scope for connection 
            int numOfRows = 0;
            try
            {
                using (var cmd = new SqlCommand("INSERT INTO Products(name, price, quantity) VALUES (@p_name, @p_price, @p_quantity)", _connection))
                {
                    cmd.Parameters.AddWithValue("@p_name", product.Name);
                    cmd.Parameters.AddWithValue("@p_price", product.Price);
                    cmd.Parameters.AddWithValue("@p_quantity", product.Quantity);

                    _connection.Open();
                    numOfRows = cmd.ExecuteNonQuery();

                }
            }
            catch (SqlException ex)
            {
                foreach (SqlError error in ex.Errors)
                {
                    Console.WriteLine($"SQL Server Error: {error.Message}");
                }
            }
            finally
            {
                _connection.Close();
            }
            return numOfRows > 0;
        }

        public IEnumerable<Product> GetAllProducts()
        {
            List<Product> products = null;

            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM Products;", _connection))
                {
                    _connection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        products = ReadProductList(reader);
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
            finally
            {
                _connection.Close();
            }

            return products;
        }

        public Product GetProduct(string productName)
        {
            Product fetchedProduct = null;
            try
            {
                // Always returns one element (name => primary key)
                using (var cmd = new SqlCommand("SELECT * FROM Products WHERE name = @p_name", _connection))
                {
                    cmd.Parameters.AddWithValue("@p_name", productName);
                    _connection.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        fetchedProduct = ReadProductList(reader)[0];
                    }
                }
            }
            catch (SqlException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                _connection.Close();
            }

            return fetchedProduct;
        }
        private List<Product> ReadProductList(SqlDataReader reader)
        {
            // move reader.read to readProduct
            List<Product> products = new List<Product>();

            while (reader.Read())
            {
                products.Add(ReadProduct(reader));
            }
            return products;
        }
        private Product ReadProduct(SqlDataReader reader)
        {
            var readProduct = new Product() { Name = reader.GetString(0), Price = (float)reader.GetDouble(1) };
            readProduct.IncreaseQuantity(reader.GetInt32(2));
            return readProduct;
        }
        public bool RemoveProduct(string productName)
        {
            int rowNum = 0;
            try
            {
                using (var cmd = new SqlCommand("DELETE FROM Products WHERE name = @p_name", _connection))
                {

                    cmd.Parameters.AddWithValue("@p_name", productName);
                    _connection.Open();
                    rowNum = cmd.ExecuteNonQuery();
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
            return rowNum > 0;
        }

        public bool UpdateProduct(string productName, Product newProduct)
        {
            return (productName == newProduct.Name) ?
                UpdateProductWithoutPrimary(productName, newProduct) :
                ReplaceProduct(productName, newProduct);
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
        private bool ReplaceProduct(string productName, Product newProduct)
        {
            // create new command for each operation 
            bool updateState = false;

            _connection.Open();
            SqlTransaction transaction = _connection.BeginTransaction();
            try
            {
                using (var sqlCommand = new SqlCommand())
                {
                    sqlCommand.Transaction = transaction;
                    sqlCommand.Connection = _connection;

                    if (!IsProductNameAvailable(newProduct.Name, sqlCommand))
                        return false;

                    DeleteProductCommand(productName, sqlCommand); // delete old product
                    InsertProductCommand(newProduct, sqlCommand); // inserts new one
                }
                transaction.Commit();
                updateState = true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine(ex);
                transaction.Rollback();
            }
            finally
            {
                transaction.Dispose();
                _connection.Close();
            }
            return updateState;
        }

        private bool IsProductNameAvailable(string name, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = "SELECT COUNT('*') FROM Products WHERE name = @p_name";
            sqlCommand.Parameters.AddWithValue("@p_name", name);

            int count = (int)sqlCommand.ExecuteScalar();
            sqlCommand.Parameters.Clear();
            return count == 0;
        }
        private bool DeleteProductCommand(string productName, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = "DELETE FROM Products WHERE name = @p_name";
            sqlCommand.Parameters.AddWithValue("@p_name", productName);

            int rowNum = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();

            return rowNum > 0;
        }

        private bool InsertProductCommand(Product newProduct, SqlCommand sqlCommand)
        {
            sqlCommand.CommandText = "INSERT INTO Products(name, price, quantity) VALUES (@p_name, @p_price, @p_quantity)";
            sqlCommand.Parameters.AddWithValue("@p_name", newProduct.Name);
            sqlCommand.Parameters.AddWithValue("@p_price", newProduct.Price);
            sqlCommand.Parameters.AddWithValue("@p_quantity", newProduct.Quantity);

            int rowNum = sqlCommand.ExecuteNonQuery();
            sqlCommand.Parameters.Clear();

            return rowNum > 0;
        }

        private bool UpdateProductCommand(string productName, Product newProduct, SqlCommand cmd)
        {
            cmd.CommandText = "UPDATE Products SET Price = @p_price, Quantity = @p_quan WHERE name = @p_name";
            cmd.Parameters.AddWithValue("@p_name", productName);
            cmd.Parameters.AddWithValue("@p_price", newProduct.Price);
            cmd.Parameters.AddWithValue("@p_quan", newProduct.Quantity);

            int rowNum = cmd.ExecuteNonQuery();
            return rowNum > 0;
        }
    }
}
