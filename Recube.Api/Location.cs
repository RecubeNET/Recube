using System;

namespace Recube.Api
{
    public struct Location
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public Location(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Location operator +(Location loc, Location loc2) =>
            new Location(loc.X + loc2.X, loc.Y + loc2.Y, loc.Z + loc2.Z);

        public static Location operator -(Location loc, Location loc2) =>
            new Location(loc.X - loc2.X, loc.Y - loc2.Y, loc.Z - loc2.Z);

        public static Location operator *(Location loc, int amount) =>
            new Location(loc.X * amount, loc.Y * amount, loc.Z * amount);

        public bool Equals(Location other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object? obj)
        {
            return obj is Location other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }
    }
}