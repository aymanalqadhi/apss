namespace APSS.Domain.Entities;

/// <summary>
/// An enum to represent the possible permissions to inherit
/// </summary>
[Flags]
public enum PermissionType
{
    Read = 1,
    Delete = 2,
    Update = 4,
    Create = 8,
}

public static class PermissionTypeExtensions
{
    public static IEnumerable<string> GetPermissionValues(this PermissionType permissions)
    {
        if (permissions.HasFlag(PermissionType.Read))
            yield return "read";

        if (permissions.HasFlag(PermissionType.Delete))
            yield return "delete";

        if (permissions.HasFlag(PermissionType.Update))
            yield return "update";

        if (permissions.HasFlag(PermissionType.Create))
            yield return "create";
    }

    public static string ToFormattedString(this PermissionType permissions)
        => $"{{ {string.Join(", ", permissions.GetPermissionValues())} }}";
}