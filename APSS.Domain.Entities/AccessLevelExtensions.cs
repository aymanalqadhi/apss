namespace APSS.Domain.Entities;

public static class AccessLevelExtensions
{
    /// <summary>
    /// Checks if this access level is above the passed level
    /// </summary>
    /// <param name="self"></param>
    /// <param name="rhs">The access level to check against</param>
    /// <returns></returns>
    public static bool IsAbove(this AccessLevel self, AccessLevel rhs)
        => (uint)self > (uint)rhs;

    /// <summary>
    /// Checks if this access level is below the passed level
    /// </summary>
    /// <param name="self"></param>
    /// <param name="rhs">The access level to check against</param>
    /// <returns></returns>
    public static bool IsBelow(this AccessLevel self, AccessLevel rhs)
        => (uint)self < (uint)rhs;

    /// <summary>
    /// Gets the next level above this level
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static AccessLevel NextLevelUpove(this AccessLevel self)
    {
        if (self == AccessLevel.Root)
            throw new InvalidOperationException("root users cannot have superusers");

        if (self == AccessLevel.Presedint)
            return AccessLevel.Root;

        return (AccessLevel)(((uint)self) << 1);
    }

    /// <summary>
    /// Gets the next level below this level
    /// </summary>
    /// <param name="self"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static AccessLevel NextLevelBelow(this AccessLevel self)
    {
        if (self == AccessLevel.Farmer)
            throw new InvalidOperationException("famers cannot have subusers");

        if (self == AccessLevel.Root)
            return AccessLevel.Presedint;

        return (AccessLevel)(((uint)self) >> 1);
    }
}