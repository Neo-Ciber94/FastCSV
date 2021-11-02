using System;
using System.IO;
using System.Text;
using FastCSV;

using var stream = new MemoryStream();
using var writter = new CsvWriter(stream, leaveOpen: true);

var options = new CsvConverterOptions
{
    NamingConvention = new UpperCaseNamingConvention()
};

writter.WriteType<Product>(options);
writter.WriteValue(new Product(1, "Red Chair", 500m), options);
writter.WriteValue(new Product(2, "Blue Toaster", 100m), options);
writter.WriteValue(new Product(3, "Green Sofa", 2350m), options);

Console.WriteLine(Encoding.UTF8.GetString(stream.ToArray()));

class UpperCaseNamingConvention : CsvNamingConvention
{
    public override string Convert(string name)
    {
        return name.ToUpper();
    }
}

record Product(int Id, string Name, decimal Price);
