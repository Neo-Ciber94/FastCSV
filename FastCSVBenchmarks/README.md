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