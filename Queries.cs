namespace Generic.Ado.Net
{
    public class Queries<T>
    {
        private readonly Type type = new Type();

        public string SelectAll() => $"SELECT * FROM {typeof(T).Name.ToUpper()};";
        public string GetById(int id) => $"SELECT * FROM {typeof(T).Name.ToUpper()} WHERE ID = {id};";

        public string SelectWhere(string table, string property, object item)
        {
            return $"SELECT * {table} WHERE {property} = {type.HandleType(item)};";
        }

    }
}