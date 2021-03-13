using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Generic.Ado.Net
{
    public class Command<T>
    {
        public string Insert(T obj)
        {
            var properties = obj.GetType().GetProperties();

            var columns = CreateColumns(properties);
            var values = CreateValues(properties);

            var insert = $"INSERT INTO {typeof(T).Name.ToUpper()} ({columns}) VALUES ({values});";

            return insert;
        }

        public string Update(T obj, string property, object isEqual)
        {
            var properties = obj.GetType().GetProperties();

            var values = CreateUpdateFields(properties);

            var update = $"UPDATE {typeof(T).Name.ToUpper()} SET {values} WHERE {property.ToUpper()} = {isEqual}";

            return update;
        }

        public string Delete(string property, object isEqual) =>
            $"DELETE FROM {typeof(T).Name.ToUpper()} WHERE {property} = {isEqual}";

        private static string CreateColumns(IEnumerable<PropertyInfo> properties)
        {
            var columns = $"{properties.Select(x => x.Name.ToUpper()).Aggregate("", (current, column) => current + $"{column},")}";
            return columns.Remove(columns.Length - 1);
        }

        private static string CreateValues(IEnumerable<PropertyInfo> properties)
        {
            var values = $"{properties.Select(x => x.Name).Aggregate("", (current, column) => current + $"@{column},")}";
            return values.Remove(values.Length - 1);
        }

        private static string CreateUpdateFields(IEnumerable<PropertyInfo> properties)
        {
            var values = $"{properties.Select(x => x.Name).Aggregate("", (current, column) => current + $"{column} = @{column},")}";
            return values.Remove(values.Length - 1);
        }
    }
}