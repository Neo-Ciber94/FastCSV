using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FastCSV
{
    public readonly struct ColumnName : IEquatable<ColumnName>
    {
        public string Name { get; }
        public string? Alias { get; }

        public ColumnName(string name)
        {
            Name = name;
            Alias = null;
        }

        public ColumnName(string name, string alias)
        {
            Name = name;
            Alias = alias;
        }

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
        public static ColumnName As(this string s, string alias)
        {
            return new ColumnName(s, alias);
        }
    }
}
