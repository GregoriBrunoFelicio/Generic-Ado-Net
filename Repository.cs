using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Generic.Ado.Net
{
    public class Repository<T>
    {

        private readonly Command<T> command = new Command<T>();
        private readonly Queries<T> queries = new Queries<T>();
        private readonly string connectionString;


        public Repository(string connection)
        {
            if (string.IsNullOrWhiteSpace(connection)) throw new NullReferenceException("The connection string could not be null or empty.");
            connectionString = connection;
        }

        public async Task<int> InsertAsync(T obj)
        {
            try
            {
                var insert = command.Insert(obj);

                await using var connection = new SqlConnection(connectionString);

                var sqlCommand = CreateSqlCommand(insert, obj);
                sqlCommand.Connection = connection;

                connection.Open();

                var affectedRows = await sqlCommand.ExecuteNonQueryAsync();

                connection.Close();

                return affectedRows;
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<int> UpdateAsync(T obj, string property, object isEqual)
        {
            try
            {
                var update = command.Update(obj, property, isEqual);

                await using var connection = new SqlConnection(connectionString);

                var sqlCommand = CreateSqlCommand(update, obj);
                sqlCommand.Connection = connection;

                connection.Open();

                var affectedRows = await sqlCommand.ExecuteNonQueryAsync();

                connection.Close();

                return affectedRows;
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }


        public async Task<int> DeleteAsync(string property, object isEqual)
        {
            var delete = command.Delete(property, isEqual);
            await using var connection = new SqlConnection(connectionString);
            var sqlCommand = new SqlCommand(delete, connection)
            {
                Connection = connection,
                CommandType = CommandType.Text
            };
            sqlCommand.Parameters.AddWithValue($"@{property}", isEqual);
            connection.Open();
            var affectedRows = await sqlCommand.ExecuteNonQueryAsync();
            connection.Close();
            return affectedRows;
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                var query = queries.SelectAll();

                await using var connection = new SqlConnection(connectionString);

                var sqlCommand = new SqlCommand(query, connection) { Connection = connection, CommandType = CommandType.Text };

                connection.Open();

                await using var sqlDataReader = await sqlCommand.ExecuteReaderAsync();

                var objects = ReadItems(sqlDataReader);

                connection.Close();

                return await objects;
            }
            catch (SqlException e)
            {
                throw new Exception(e.Message);
            }
        }

        private static async Task<IEnumerable<T>> ReadItems(DbDataReader dataReader)
        {
            var list = new List<T>();

            while (await dataReader.ReadAsync())
            {
                var obj = Activator.CreateInstance<T>();
                for (var i = 0; i < dataReader.FieldCount; i++)
                {
                    var propertyName = dataReader.GetName(i);
                    var value = dataReader[i];
                    obj.GetType().GetProperty(propertyName)?.SetValue(obj, value);
                }
                list.Add(obj);
            }

            return list;
        }

        private static SqlCommand CreateSqlCommand(string sqlCommand, T obj)
        {
            var commend = new SqlCommand(sqlCommand) { CommandType = CommandType.Text };

            foreach (var property in obj.GetType().GetProperties().Select(x => x))
                commend.Parameters.AddWithValue($"@{property.Name}", (property.GetValue(obj)));

            return commend;
        }
    }
}