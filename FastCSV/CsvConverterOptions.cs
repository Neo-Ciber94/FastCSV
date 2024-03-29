﻿using FastCSV.Converters;
using FastCSV.Internal;
using System;
using System.Collections.Generic;

namespace FastCSV
{
    /// <summary>
    /// Options used for serialize or deserialize a csv.
    /// </summary>
    public record CsvConverterOptions
    {
        /// <summary>
        /// A set of default <see cref="CsvConverterOptions"/> options.
        /// </summary>
        public static CsvConverterOptions Default { get; } = new CsvConverterOptions();

        /// <summary>
        /// Format used for the serialization or deserialization.
        /// </summary>
        public CsvFormat Format { get; init; } = CsvFormat.Default;

        /// <summary>
        /// If <c>true</c> class fields will be included during serialization/deserialization, by default only properties are included.
        /// Default is <c>false</c>.
        /// </summary>
        public bool IncludeFields { get; init; } = false;

        /// <summary>
        /// If <c>true</c> a header will be included during serialization/deserialization.
        /// Default is <c>true</c>.
        /// </summary>
        public bool IncludeHeader { get; init; } = true;

        /// <summary>
        /// Whether if match all the columns in the csv with the properties of the target.
        /// </summary>
        public bool MatchExact { get; init; } = false;

        /// <summary>
        /// Naming convention used.
        /// </summary>
        public CsvNamingConvention? NamingConvention { get; init; }

        /// <summary>
        /// Defines how handle nested objects.
        /// </summary>
        public NestedObjectHandling? NestedObjectHandling { get; init; }

        /// <summary>
        /// Defines how handle a collection of objects.
        /// </summary>
        public CollectionHandling? CollectionHandling { get; init; }

        /// <summary>
        /// A list of custom <see cref="ICsvValueConverter"/>.
        /// </summary>
        public IReadOnlyList<ICsvValueConverter> Converters { get; init; } = Array.Empty<ICsvValueConverter>();

        /// <summary>
        /// A list of custom <see cref="ITypeGuesser"/> used for determine the type to deserialize a string value when the source property is an <see cref="object"/>.
        /// 
        /// <para>
        /// If there is not a builtin converter for the given type, you need to provide a custom converter as well.
        /// </para>
        /// </summary>
        public IReadOnlyList<ITypeGuesser> TypeGuessers { get; init; } = Array.Empty<ITypeGuesser>();

        /// <summary>
        /// The <see cref="CsvConverterProvider"/> used for this option.
        /// </summary>
        public CsvConverterProvider ConverterProvider { get; init; } = CsvConverterProvider.Default;

        /// <summary>
        /// Provider for reflection operations.
        /// </summary>
        public IReflector ReflectionProvider { get; init; } = CachedReflector.Default;

        /// <summary>
        /// The delimiter of the format.
        /// </summary>
        public string Delimiter => Format.Delimiter;

        /// <summary>
        /// The quote of the format.
        /// </summary>
        public string Quote => Format.Quote;

        /// <summary>
        /// The quote style of the format.
        /// </summary>
        public QuoteStyle Style => Format.Style;

        /// <summary>
        /// Whether ignore or not whitespaces when deserializing.
        /// </summary>
        public bool IgnoreWhitespace => Format.IgnoreWhitespace;
    }
}