# Benchmarks

### Legends
|  Legend    |   Description                       |
|------------|-------------------------------------|
|  Mean      | Arithmetic mean of all measurements |
|  Error     | Half of 99.9% confidence interval |
|  StdDev    | Standard deviation of all measurements |
|  Min       | Minimum |
|  Max       | Maximum |
|  Ratio     | Mean of the ratio distribution ([Current]/[Baseline]) |
|  RatioSD   | Standard deviation of the ratio distribution ([Current]/[Baseline]) |
|  Gen 0     | GC Generation 0 collects per 1000 operations |
|  Gen 1     | GC Generation 1 collects per 1000 operations |
|  Gen 2     | GC Generation 2 collects per 1000 operations |
|  Allocated | Allocated memory per single operation (managed only, inclusive, 1KB = 1024B) |
|  1 ms      | 1 Millisecond (0.001 sec) |

## FastCSV 1.0.0

|       Method |     Mean |     Error |    StdDev |      Min |      Max | Ratio | RatioSD |    Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------- |---------:|----------:|----------:|---------:|---------:|------:|--------:|---------:|------:|------:|----------:|
|      ReadAll | 1.905 ms | 1.7905 ms | 0.0981 ms | 1.836 ms | 2.017 ms |  1.00 |    0.00 | 201.1719 |     - |     - | 824.27 KB |
| ReadAllAsync | 1.883 ms | 0.8982 ms | 0.0492 ms | 1.853 ms | 1.939 ms |  0.99 |    0.06 | 199.2188 |     - |     - |  824.3 KB |

## FastCSV CsvReader.ReadAllAs and CsvReaderReadAllAs with struct enumerator

|                     Method |     Mean |    Error |   StdDev |      Min |      Max | Ratio | RatioSD |     Gen 0 |    Gen 1 |   Gen 2 | Allocated |
|--------------------------- |---------:|---------:|---------:|---------:|---------:|------:|--------:|----------:|---------:|--------:|----------:|
|                    ReadAll | 20.84 ms | 0.413 ms | 0.765 ms | 19.36 ms | 22.53 ms |  1.00 |    0.00 | 4062.5000 | 281.2500 | 62.5000 |  16.94 MB |
|      ReadAllWithEnumerator | 18.88 ms | 0.364 ms | 0.533 ms | 17.41 ms | 19.71 ms |  0.90 |    0.05 | 4218.7500 |        - |       - |  16.92 MB |
|               ReadAllAsync | 21.00 ms | 0.412 ms | 0.550 ms | 19.92 ms | 22.19 ms |  1.01 |    0.04 | 4343.7500 |        - |       - |  17.42 MB |
| ReadAllWithAsyncEnumerator | 21.20 ms | 0.422 ms | 1.156 ms | 18.62 ms | 23.79 ms |  1.03 |    0.07 | 4218.7500 |        - |       - |  16.92 MB |