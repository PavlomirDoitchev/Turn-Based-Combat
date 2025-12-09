using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int x, z;
    public GridPosition(int x, int z)
    {
        this.x = x;
        this.z = z;
    }

    public override string ToString()
    {
        return $"X: {x}, Z: {z}";
    }
  

    /// <summary>
    /// Override equality operator
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(GridPosition a, GridPosition b)
    {
        return a.x == b.x && a.z == b.z;
    }
    /// <summary>
    /// Override inequality operator
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(GridPosition a, GridPosition b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               x == position.x &&
               z == position.z;
    }

    public static GridPosition operator +(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x + b.x, a.z + b.z);
    }
    public static GridPosition operator -(GridPosition a, GridPosition b)
    {
        return new GridPosition(a.x - b.x, a.z - b.z);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(x, z);
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }
}