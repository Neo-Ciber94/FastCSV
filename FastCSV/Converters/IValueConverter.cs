
namespace FastCSV.Converters
{
    public interface IValueConverter
    {
        public bool TryParse(string? s, out object? value);

        public string ToValue(object? value);
    }

    public interface IValueConverter<T> : IValueConverter
    {
        public bool TryParse(string? s, out T value);
        public string ToValue(T value);

        string IValueConverter.ToValue(object? value)
        {
            return ToValue((T)value!);
        }

        bool IValueConverter.TryParse(string? s, out object? value)
        {
            value = null;

            if (TryParse(s, out T result))
            {
                value = result;
            }

            return value != null;
        }
    }
}
