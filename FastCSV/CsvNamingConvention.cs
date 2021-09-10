using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastCSV.Utils;

namespace FastCSV
{
    public abstract class CsvNamingConvention
    {
        public static CsvNamingConvention SnakeCase { get; } = new SnakeCaseNamingConvention();

        public abstract string Convert(string name);
    }

    internal class SnakeCaseNamingConvention : CsvNamingConvention
    {
        enum LetterCase
        {
            Lower, Upper, Unknown
        }

        public override string Convert(string name)
        {
            int length = name.Length;

            if (length == 0)
            {
                return name;
            }

            StringBuilder stringBuilder = StringBuilderCache.Acquire(name.Length);
            LetterCase letterCase = LetterCase.Unknown;

            for (int i = 0; i < length; i++)
            {
                char c = name[i];
                LetterCase currentCase = GetLetterCase(c);

                if (letterCase == LetterCase.Lower && currentCase == LetterCase.Upper)
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(char.ToLower(c));
                    letterCase = LetterCase.Unknown;
                }
                else if (letterCase == LetterCase.Upper && currentCase == LetterCase.Lower)
                {
                    int nextIndex = i + 1;
                    bool nextIsUpper = nextIndex < length && GetLetterCase(name[nextIndex]) == LetterCase.Upper;

                    stringBuilder.Append(char.ToLower(c));

                    if (nextIsUpper)
                    {
                        stringBuilder.Append('_');
                        letterCase = LetterCase.Unknown;
                    }
                }
                else
                {
                    stringBuilder.Append(char.ToLower(c));
                    letterCase = currentCase;
                }
            }

            return StringBuilderCache.ToStringAndRelease(ref stringBuilder!);

            // Helper
            static LetterCase GetLetterCase(char letter)
            {
                return letter switch
                {
                    char c when char.IsUpper(c) => LetterCase.Upper,
                    char c when char.IsLower(c) => LetterCase.Lower,
                    _ => LetterCase.Unknown
                };
            }
        }
    }
}
