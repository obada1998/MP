namespace Platform.Application.Auth;

public static class PlatformRoles
{
    public const string PlatformAdmin = nameof(PlatformAdmin);
    public const string StoreOwner = nameof(StoreOwner);
    public const string StoreStaff = nameof(StoreStaff);
    public const string Customer = nameof(Customer);

    public static readonly string[] All =
    [
        PlatformAdmin,
        StoreOwner,
        StoreStaff,
        Customer
    ];
}
