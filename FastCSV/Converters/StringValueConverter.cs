
namespace FastCSV.Converters
{
    public class StringValueConverter : IValueConverter<string>
    {
        public string ToValue(string value)
        {
            return value;
        }

        public bool TryParse(string? s, out string value)
        {
            value = s?? string.Empty;
            return s != null;
        }
    }
}
