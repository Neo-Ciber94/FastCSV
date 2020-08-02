using System;

namespace FastCSV.Utils
{
    /// <summary>
    /// Represents a position in a file.
    /// </summary>
    /// <seealso cref="System.IEquatable{FastCSV.Utils.Position}" />
    [Serializable]
    public readonly struct Position : IEquatable<Position>
    {
        public static Position Zero => new Position();

        /// <summary>
        /// Initializes a new instance of the <see cref="Position"/> struct.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <param name="offset">The offset.</param>
        public Position(int line, int offset)
        {
            if(line < 0)
            {
                throw new ArgumentException(nameof(line));
            }

            if(offset < 0)
            {
                throw new ArgumentException(nameof(offset));
            }

            Line = line;
            Offset = offset;
        }

        /// <summary>
        /// Gets the current line.
        /// </summary>
        /// <value>
        /// The line.
        /// </value>
        public int Line { get; }

        /// <summary>
        /// Gets the offset from the current line.
        /// </summary>
        /// <value>
        /// The offset.
        /// </value>
        public int Offset { get; }

        /// <summary>
        /// Gets a copy of this position with the given line.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns></returns>
        public Position WithLine(int line)
        {
            return new Position(line, this.Offset);
        }

        /// <summary>
        /// Gets a copy of this position with the given offset from the current line.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public Position WithOffset(int offset)
        {
            return new Position(this.Line, offset);
        }

        /// <summary>
        /// Adds offset to the current position.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public Position AddOffset(int offset)
        {
            return new Position(this.Line, this.Offset + offset);
        }

        /// <summary>
        /// Adds the line offset.
        /// </summary>
        /// <param name="lineOffset">The line offset.</param>
        /// <returns></returns>
        public Position AddLine(int line)
        {
            return new Position(this.Line + line, this.Offset);
        }

        public override string ToString()
        {
            return $"{{{nameof(Line)}={Line}, {nameof(Offset)}={Offset}}}";
        }

        public override bool Equals(object? obj)
        {
            return obj is Position position && Equals(position);
        }

        public bool Equals(Position other)
        {
            return Line == other.Line &&
                   Offset == other.Offset;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Line, Offset);
        }

        public static bool operator ==(Position left, Position right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Position left, Position right)
        {
            return !(left == right);
        }
    }
}
