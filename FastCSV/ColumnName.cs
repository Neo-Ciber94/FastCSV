using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastCSV
{
    /// <summary>
    /// Represents the name of a column in a csv file.
    /// </summary>
    /// <seealso cref="System.IEquatable{FastCSV.ColumnName}" />
    public readonly struct ColumnName : IEquatable<ColumnName>
    {
        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        /// Gets the alias name of the column.
        /// </summary>
        /// <value>
        /// The alias.
        /// </value>
        public string? Alias { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnName"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        public ColumnName(string name)
        {
            Name = name;
            Alias = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnName"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="alias">The alias.</param>
        public ColumnName(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

        /// <summary>
        /// Gets the alias name of this column or the name if don't have an alias.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string GetAliasOrName()
        {
            return Alias ?? Name;
        }

        public override string ToString()
        {
            return $"{{{nameof(Name)}={Name}, {nameof(Alias)}={Alias}}}";
        }

        public static implicit operator ColumnName(string s) => new ColumnName(s, s);

        public static bool operator ==(ColumnName left, ColumnName right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ColumnName left, ColumnName right)
        {
            return !(left == right);
        }

        public override bool Equals(object? obj)
        {
            return obj is ColumnName name && Equals(name);
        }

        public bool Equals(ColumnName other)
        {
            return Name == other.Name &&
                   Alias == other.Alias;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Alias);
        }
    }

    public static class ColumnNameExtensions
    {
        /// <summary>
        /// Converts this string to a <see cref="ColumnName"/> with the specified alias.
        /// </summary>
        /// <param name="s">The string</param>
        /// <param name="alias">The alias.</param>
        /// <returns>A column name with the specified alias.</returns>
        public static ColumnName As(this string s, string alias)
        {
            return new ColumnName(s, alias);
        }
    }
}
