using System;

namespace Generic.Ado.Net
{
    public class Type
    {
        public object HandleType(object obj)
        {
            var type = obj.GetType().Name;

            if (type.Equals(nameof(String)))
            {
                return $"'{obj}'";
            }

            return obj;
        }

    }
}
