namespace Platform.Domain.Enums;

public enum StoreMembershipRole
{
    StoreOwner = 1,
    StoreStaff = 2
}

public enum ProductStatus
{
    Draft = 1,
    Active = 2,
    Archived = 3
}

public enum ProductFieldType
{
    Text = 1,
    Number = 2,
    Money = 3,
    Boolean = 4,
    Date = 5,
    Select = 6,
    MultiSelect = 7,
    Color = 8,
    Json = 9
}

public enum PageLayoutStatus
{
    Draft = 1,
    Published = 2,
    Archived = 3
}

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Paid = 3,
    Fulfilled = 4,
    Cancelled = 5
}
