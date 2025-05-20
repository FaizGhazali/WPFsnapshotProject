using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static SnapShotHelper.DBconnection;

namespace SnapShotHelper
{
    public interface IDBconnection
    {
        public List<T> GetAllRecords<T>(string tableName) where T : new();
        

    }
    public class DBconnection : IDBconnection
    {
        private readonly string _connectionString;
        private readonly bool _debugMode;
        public DBconnection(string projectname, string projectfolder)
        {


            var dictFromRegistry = RegistryConfigurator.GetAllRegistry(projectname, projectfolder);
            _connectionString = dictFromRegistry["connectionString"].ToString() ?? string.Empty;
            _debugMode = dictFromRegistry["debug"] is int intValue
             ? intValue != 0
             : bool.TryParse(dictFromRegistry["debug"]?.ToString(), out var result) ? result : false;
        }
        public List<T> GetAllRecords<T>(string tableName) where T : new()
        {
            List<T> records = new List<T>();
            string query = $"SELECT * FROM {tableName}";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Create a new instance of T for each row
                        T model = new T();

                        // Using reflection to map columns to properties
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var property = typeof(T).GetProperty(columnName);

                            if (property != null && reader[columnName] != DBNull.Value)
                            {
                                var value = reader[columnName];

                                // Check if the property type and column value type match, otherwise convert
                                if (property.PropertyType.IsAssignableFrom(value.GetType()))
                                {
                                    property.SetValue(model, value);
                                }
                                else
                                {
                                    try
                                    {
                                        var convertedValue = Convert.ChangeType(value, property.PropertyType);
                                        property.SetValue(model, convertedValue);
                                    }
                                    catch (InvalidCastException)
                                    {
                                        // Handle or log the error if conversion is not possible
                                        Console.WriteLine($"Could not convert {value} to {property.PropertyType}");
                                    }
                                }
                            }
                        }

                        records.Add(model); // Add the fully populated model to the list
                    }
                }
            }

            return records;
        }
        //public string ConnectionString => _connectionString;
    }
}
